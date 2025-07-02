using FUNewsManagement.Models;
using System.Text.Json;

namespace StudentNameRazorPages.Helpers;

public static class SessionHelper
{
    private const string USER_SESSION_KEY = "CurrentUser";

    public static void SetCurrentUser(ISession session, SystemAccount user)
    {
        var userJson = JsonSerializer.Serialize(user);
        session.SetString(USER_SESSION_KEY, userJson);
    }

    public static SystemAccount? GetCurrentUser(ISession session)
    {
        var userJson = session.GetString(USER_SESSION_KEY);
        if (string.IsNullOrEmpty(userJson))
            return null;

        return JsonSerializer.Deserialize<SystemAccount>(userJson);
    }

    public static void ClearCurrentUser(ISession session)
    {
        session.Remove(USER_SESSION_KEY);
    }

    public static bool IsLoggedIn(ISession session)
    {
        return !string.IsNullOrEmpty(session.GetString(USER_SESSION_KEY));
    }

    public static bool IsAdmin(ISession session)
    {
        var user = GetCurrentUser(session);
        return user?.AccountRole == 0; // Admin role
    }

    public static bool IsStaff(ISession session)
    {
        var user = GetCurrentUser(session);
        return user?.AccountRole == 1; // Staff role
    }

    public static bool IsLecturer(ISession session)
    {
        var user = GetCurrentUser(session);
        return user?.AccountRole == 2; // Lecturer role
    }

    public static string GetUserName(ISession session)
    {
        var user = GetCurrentUser(session);
        return user?.AccountName ?? "Guest";
    }

    public static short GetUserId(ISession session)
    {
        var user = GetCurrentUser(session);
        return user?.AccountID ?? 0;
    }

    public static int GetUserRole(ISession session)
    {
        var user = GetCurrentUser(session);
        return user?.AccountRole ?? -1;
    }
}
