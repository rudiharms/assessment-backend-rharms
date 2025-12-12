using System.Reflection;

namespace Assecor.Api.Application;

public static class Application
{
    public static readonly Assembly Assembly = typeof(Application).Assembly;
}
