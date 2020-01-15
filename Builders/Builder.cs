using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Builders
{
    public interface IBuilder<TItem> where TItem : new()
    {
        TItem BuildAndSave();
    }

    public abstract class Builder<T, TItem> : IBuilder<TItem> where T : Builder<T, TItem>
        where TItem : new()
    {
        protected readonly SchoolManagementContext Context;
        protected TItem State;

        protected Builder()
        {
            Init();
        }

        protected Builder(SchoolManagementContext context)
        {
            Context = context;
            Init();
        }

        public virtual TItem Build()
        {
            return State;
        }

        public virtual TItem BuildAndSave()
        {
            Save();
            return State;
        }

        public T Save()
        {
            var entity = State as IEntity;
            var identityUser = State as IdentityUser;
            if (entity != null)
            {
                if (Context.Entry(entity).State != EntityState.Modified)
                {
                    Context.Add(entity);
                }
            }
            else if (identityUser != null)
            {
                Context.Add(identityUser);
            }
            else if(State is IdentityRole)
            {
                Context.Add(State);
            }
            else
            {
                throw new BuilderSaveException($"State {State} has to be an Entity or IdentityUser.");
            }

            Context.SaveChanges();
            return (T)this;
        }

        public T With(Action<TItem> operation)
        {
            operation.Invoke(State);
            return (T)this;
        }

        private void Init()
        {
            State = new TItem();
        }

        public TItem Get()
        {
            return State;
        }

        public TItem As(TItem item)
        {
            return State = item;
        }

        public T For(Action<T> operation)
        {
            operation.Invoke((T)this);
            return (T)this;
        }

    }

    public class BuilderSaveException : Exception
    {
        public BuilderSaveException(string msg) : base(msg)
        {
        }
    }
}
