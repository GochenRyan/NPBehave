﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using NPBehave;

namespace NPBehave
{
    // IAUS https://archive.org/details/GDC2013Mark > From min 30

    public enum InputType
    {
        Blackboard,
        IdleTime
    }

    public class UtilityInput
    {
        public InputType InputType;
        public string InputKey;

        public float GetValue( UtilitySelector selector )
        {
            if( InputType == InputType.Blackboard )
            {
               return selector.Blackboard.Get<float>( InputKey );
            }
            else
            {
                return selector.GetUtilityAction( InputKey ).IdleTime;
            }
        }
    }

    public class UtilityNormalizer
    {
        // bookends, min
        public float Min;
        // bookends, max
        public float Max;

        public float Normalize( float input )
        {
            return MathHelper.Clamp( ( input - Min ) / ( Max - Min ), 0.0f, 1.0f );
        }
    }

    public enum CurveType
    {
        LinearQuad,
        Logistic,
        Logit
    }

    public class UtilityCurve
    {
        public CurveType CurveType;
        public float CurveM; // slope
        public float CurveK; // expoent
        public float CurveB; // y-intercept ( vertical shift )
        public float CurveC; // x-intercept ( horizontal shift )
         
        public float CalculateScore( float normalized )
        {
            float value = normalized;
            // https://archive.org/details/GDC2013Mark ~ min 40
            if ( CurveType == CurveType.LinearQuad )
            {
                // CurveM; // slope
                // CurveK; // expoent
                // CurveB; // y-intercept ( vertical shift )
                // CurveC; // x-intercept ( horizontal shift )
                value = CurveM * (float)Math.Pow( normalized - CurveC, CurveK ) + CurveB;
            }
            else if ( CurveType == CurveType.Logistic )
            {
                // CurveM; // slope at the inflection point
                // CurveK; // vertical size of the curve
                // CurveB; // y-intercept ( vertical shift )
                // CurveC; // x-interception of the inflection point ( horizontal shift )
                value = CurveK * ( 1.0f / ( 1 + (float)Math.Pow( ( 1000 * Math.E * CurveM ), -1 * normalized + CurveC ) ) ) + CurveB;
            }
            return MathHelper.Clamp( value, 0.0f, 1.0f );
        }
    }

    public class UtilityConsideration
    {
        public UtilityInput Input;
        public UtilityNormalizer Normalizer;
        public UtilityCurve Curve;

        public float LastNormalized = 0.0f;
        public float LastScore = 0.0f;

        public float CalculateUtility(UtilitySelector selector)
        {
            float rawValue = Input.GetValue(selector);
            float normalized = Normalizer.Normalize( rawValue );
            float score = Curve.CalculateScore( normalized );
            LastScore = score;
            LastNormalized = normalized;
            return score;
        }
    }

    public class UtilityAction
    {
        public UtilitySelector Selector;

        public string Name;
        public Node Subtree;
        public List<UtilityConsideration> Considerations;
        public float Weight = 1.0f;
        public float AbortThreshold = 0.0f;

        public float LastUtility;
        public double LastTimestamp = 0.0f;


        public float IdleTime
        {
            get
            {
                return (float) ( Selector.Clock.ElapsedTime - LastTimestamp );
            }
        }

        public void SetSelector( UtilitySelector selector )
        {
            this.Selector = selector;
        }

        public float CalculateScore( float min )
        {
            //https://archive.org/details/GDC2015Mark -> Min 10:00
            int numConsiderations = Considerations.Count;
            float modificationFactor = 1.0f - ( 1.0f / ( float ) numConsiderations );

            float totalScore = Weight;

            foreach ( UtilityConsideration axis in Considerations )
            {
                if( totalScore < min )
                {
                    return 0.0f;
                }
                float score = axis.CalculateUtility(Selector);
                float makeUpValue = ( 1.0f - score ) * modificationFactor;
                float finalScore = score + ( makeUpValue * score );
                totalScore *= finalScore;
            }

            LastUtility = totalScore;
            return totalScore;
        }
    }

    public class UtilitySelector : Composite
    {
        private UtilityAction lastStopped = null;
        private UtilityAction current = null;
        public readonly UtilityAction[] Actions;
        private Dictionary<string, UtilityAction> ActionsDict = new Dictionary<string, UtilityAction>();

        public float UpdateInterval = 0.25f;

        public UtilitySelector( params UtilityAction[] actions ) : base( "Utility" )
        {
            Children = new Node[ actions.Length ];
            for( int i = 0; i < Children.Length; i++  )
            {
                Children[ i ] = actions[ i ].Subtree;
                Children[ i ].SetParent( this );
                ActionsDict[ actions[ i ].Name ] = actions[ i ];
            }
            Actions = actions;
        }

        public override void SetRoot( Root rootNode )
        {
            base.SetRoot( rootNode );
            foreach ( UtilityAction action in Actions )
            {
                action.SetSelector( this );
            }
        }

        public UtilityAction GetActiveAction()
        {
            return current;
        }

        public UtilityAction GetUtilityAction( string name )
        {
            return ActionsDict.ContainsKey(name) ? ActionsDict[ name ] : null;
        }

        protected override void DoStart()
        {
            foreach ( Node child in Children )
            {
                Debug.Assert( child.CurrentState == State.INACTIVE );
            }

            current = null;

            ProcessChildren();
        }

        protected override void DoStop()
        {
            current.Subtree.Stop();
        }

        protected override void DoChildStopped( Node child, bool result )
        {
            //// utility doesn't care about the result
            current.LastTimestamp = Clock.ElapsedTime;
            lastStopped = current;
            current = null;
            Clock.RemoveTimer( ObserveUtilities );
            ProcessChildren();
        }

        private void ProcessChildren()
        {
            if ( IsStopRequested )
            {
                Stopped( false );
                return;
            }

            UtilityAction selected = null;
            float fBestUtility = 0.0f;
            foreach ( UtilityAction action in Actions )
            {
                if( action == lastStopped )
                {
                    continue;
                }

                float utility = action.CalculateScore( fBestUtility );
                if( utility > fBestUtility )
                {
                    fBestUtility = utility;
                    selected = action;
                }
            }

            lastStopped = null;

            if( null == selected )
            {
                Stopped( false );
                return;
            }
            else
            {
                current = selected;
                Clock.AddTimer( UpdateInterval, -1, ObserveUtilities );
                current.Subtree.Start();
            }
        }

        private void ObserveUtilities()
        {
            UtilityAction selected = null;
            float fBestUtility = 0.0f;
            float fCurrentUtility = 0.0f;
            foreach ( UtilityAction action in Actions )
            {
                Debug.Assert( action.Selector == this );

                float utility = action.CalculateScore( fBestUtility );
                if ( utility > fBestUtility )
                {
                    fBestUtility = utility;
                    selected = action;
                }
                if ( action == current )
                {
                    fCurrentUtility = fBestUtility;
                }
            }
            Debug.Assert( current != null );
            if( selected != current && fCurrentUtility < current.AbortThreshold )
            {
                current.Subtree.Stop();
            }
        }

        public override void StopLowerPriorityChildrenForChild( Node abortForChild, bool immediateRestart )
        {
            // not implemented, the utility selector is the only one aborting stuffs
        }

        override public string ToString()
        {
            return base.ToString() + "[" + ( this.current != null ? this.current.Subtree.Name : "inactive" ) + "]";
        }
    }
}
