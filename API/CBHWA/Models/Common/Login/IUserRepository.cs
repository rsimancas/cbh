using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IUserRepository
    {
        IList<User> GetAll(int page, int start, int limit, ref int totalRecord);
        User Get(string id);
        User Add(User user);
        void Remove(string id);
        bool Update(User user);
        User ValidLogon(string userName, string userPassword);
        string GenToken(User usr);
    }
}
