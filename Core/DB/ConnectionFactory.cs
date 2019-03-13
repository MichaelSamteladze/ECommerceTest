using System.Configuration;

namespace Core.DB
{
    public static class ConnectionFactory
    {
        #region Properties
        static string DBConnectionString => ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;        
        #endregion

        #region Methods
        public static DBCoreDataContext GetDBCoreDataContext()
        {            
            return new DBCoreDataContext(DBConnectionString);
        }        
        #endregion
    }    
}
