using System.Collections.Generic;

class StateNode
{
    public IState State { get; }
    public HashSet<ITransition> Transitions { get; }

    public StateNode(IState state)
    {
        this.State = state;
        this.Transitions = new HashSet<ITransition>();
    }

    public void AddTransition(IState to, IPredicate condition)
    {
        Transitions.Add(new Transition(to, condition));
    }
}

