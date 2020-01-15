using MediatR;

namespace SchoolManagement.Infrastructure
{
    public class DataResult
    {
        public enum ResultStatus
        {
            Success, 
            Warning,
            Error
        }

        protected DataResult() { }
        public string Message { get; set; }
        public ResultStatus Status { get; set; }

        public static DataResult Error(string message)
        {
            return new DataResult()
            {
                Message = message,
                Status = ResultStatus.Error
            };
        }

        public static DataResult Success()
        {
            return new DataResult
            {
                Status = ResultStatus.Success
            };
        }

        public static DataResult Warning(string message)
        {
            return new DataResult
            {
                Status = ResultStatus.Warning
            };
        }
    }

    public class DataResult<TData> : DataResult, IRequest<Unit> where TData : new() 
    {
        private DataResult()
        {
            Data = new TData();
        }

        public TData Data { get; set; }

        public static DataResult<TData> Success(TData data)
        {
            return new DataResult<TData>
            {
                Data = data,
                Status = ResultStatus.Success
            };
        }

        public new static DataResult<TData> Error(string message)
        {
            return new DataResult<TData>
            {
                Message = message,
                Status = ResultStatus.Error
            };
        }
    }
}
