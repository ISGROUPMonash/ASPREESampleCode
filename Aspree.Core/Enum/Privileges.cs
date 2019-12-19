using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.Enum
{
    public enum Privileges
    {
        View_Role,
        Create_Role,
        Edit_Role,
        Delete_Role,

        View_User,
        Create_User,
        Edit_User,
        Delete_User,

        View_Entity,
        Create_Entity,
        Edit_Entity,
        Delete_Entity,
        Search_Entity,

        View_Project,
        Create_Project,
        Edit_Project,
        Delete_Project,
        Publish_Project,

        View_Forms,
        Create_Forms,
        Edit_Forms,
        Delete_Forms,

        View_Variables,
        Create_Variables,
        Edit_Variables,
        Delete_Variables,

        View_Activity,
        Create_Activity,
        Edit_Activity,
        Delete_Activity,

        View_Data_Entry,
        Create_Data_Entry,
        Edit_Data_Entry,
        Delete_Data_Entry,

        View_Email_Template,
        Create_Email_Template,
        Edit_Email_Template,
        Delete_Email_Template,
    }


    public enum AspreeDatabaseType
    {
        SQL = 1,
        Mongo = 2,
    }
}
