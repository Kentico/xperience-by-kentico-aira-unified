using CMS.Membership;

using Kentico.Membership;

namespace Kentico.Xperience.Aira.Membership;

public class Member : ApplicationUser
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FullName =>
        (FirstName, LastName) switch
        {
            ("", "") => "",
            (string first, "") => first,
            ("", string last) => last,
            (string first, string last) => $"{first} {last}",
            (null, null) or _ => "",
        };
    public bool AllowPublicProfile { get; set; } = false;
    public DateTime Created { get; set; }

    public override void MapToMemberInfo(MemberInfo target)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        /*
         * base.MapToMemberInfo will set target.MemberPassword everytime
         * however we do not want to set it if PasswordHash is null,
         * and this stores the original so we can revert it
         */
        string originalPasswordHash = target.MemberPassword;

        base.MapToMemberInfo(target);

        if (PasswordHash is null)
        {
            target.MemberPassword = originalPasswordHash;
        }

        _ = target.SetValue("MemberFirstName", FirstName);
        _ = target.SetValue("MemberLastName", LastName);
        _ = target.SetValue("MemberAllowPublicProfile", AllowPublicProfile);
    }

    public override void MapFromMemberInfo(MemberInfo source)
    {
        base.MapFromMemberInfo(source);

        FirstName = source.GetValue("MemberFirstName", "");
        LastName = source.GetValue("MemberLastName", "");
        AllowPublicProfile = source.GetValue("MemberAllowPublicProfile", false);
        Created = source.MemberCreated;
    }

    public static Member FromMemberInfo(MemberInfo memberInfo)
    {
        var member = new Member();
        member.MapFromMemberInfo(memberInfo);

        return member;
    }
}

public static class MemberInfoExtensions
{
    public static Member AsMember(this MemberInfo member) =>
        Member.FromMemberInfo(member);
}
