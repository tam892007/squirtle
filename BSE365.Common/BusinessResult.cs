using System;

namespace BSE365.Common
{
    public class BusinessResult<T> : IDisposable
    {
        private bool disposed;

        public BusinessResult()
        {
            this.IsSuccessful = false;
        }

        public bool IsSuccessful { get; set; }

        public bool HasErrors
        {
            get
            {
                return Exception != null;
            }
        } 

        public Exception Exception { get; set; }

        public T Result { get; set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {

            if (!this.disposed && disposing)
            {
                this.Result = default(T);
            }

            this.disposed = true;
        }
    }
}
