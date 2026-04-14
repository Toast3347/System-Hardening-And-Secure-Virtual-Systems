namespace ComicRealmBE.Models.Enums
{
    /// <summary>
    /// User roles for RBAC (Role-Based Access Control)
    /// 
    /// Hierarchy:
    /// - SuperAdmin: Can create Admins, manage everything
    /// - Admin: Can create Friends, manage comics and users
    /// - Friend: Can view comics only
    /// - Visitor: No database access (not a user)
    /// </summary>
    public enum UserRole
    {
        SuperAdmin = 0,
        Admin = 1,
        Friend = 2
    }
}
