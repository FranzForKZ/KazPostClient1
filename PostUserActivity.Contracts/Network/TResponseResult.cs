using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostUserActivity.Contracts.Network
{
    public class ResponseResult
    {
        public ResponseResult(RequestResultType rType)
        {
            RequestResult = rType;
        }

        public RequestResultType RequestResult { get; private set; }
    }


    public class TResponseResult<T>: ResponseResult
    {
        public TResponseResult(T result, RequestResultType rType):base(rType)
        {
            Result = result;            
        }
        public T Result { get; private set; }        
    }
}
