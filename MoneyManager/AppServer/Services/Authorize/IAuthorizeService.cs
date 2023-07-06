using AppServer.Common;
using AppServer.RequestModel;

namespace AppServer.Services;

public interface IAuthorizeService
{
    Task<Response> SignUp(RegisterRequest request);
}