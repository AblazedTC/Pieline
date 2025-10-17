using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace POSApp.Data
{
    [BsonIgnoreExtraElements] // Ignore the MongoDB _id field since its not possible to remove
    public class User
    {
        [BsonElement("phone")]          public string Phone { get; set; } = "";
        [BsonElement("email")]          public string Email { get; set; } = "";
        [BsonElement("full_name")]      public string FullName { get; set; } = "";
        [BsonElement("password_plain")] public string PasswordPlain { get; set; } = "";
        [BsonElement("user_type")]      public string UserType { get; set; } = "";
    }
}
