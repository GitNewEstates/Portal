namespace Portal_MVC.Models
{
    public class BaseClass
    {
        public BaseClass()
        {
            APIError = new APIError(ErrorType.None);
        }
        public APIError APIError { get; set; }

        private int _id;
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
             
            }
        }

        public long DocinstanceID { get; set; }

        private bool _HasError;
        public bool HasError
        {
            get
            {
                return _HasError;
            }
            set
            {
                _HasError = value;
            
            }
        }

        public int CreatedBy { get; set; }
    }
}