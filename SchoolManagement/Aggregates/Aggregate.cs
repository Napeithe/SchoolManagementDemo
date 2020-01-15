namespace SchoolManagement.Aggregates
{
    public abstract class Aggregate<TAggregate, TItem> where TItem : new()
                                                        where TAggregate : Aggregate<TAggregate, TItem>, new()
    {
        public TItem State { get; protected set; }

        protected Aggregate()
        {
            State = new TItem();
        }

        public static TAggregate New()
        {
            return new TAggregate();
        }

        public static TAggregate FromState(TItem state)
        {
            return new TAggregate
            {
                State = state
            };
        }
    }
}
