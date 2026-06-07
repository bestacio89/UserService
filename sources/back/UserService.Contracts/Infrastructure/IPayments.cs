using Franz.Common.DependencyInjection;
using UserService.Contracts.DTOs;
using Refit;
using System.Threading.Tasks;

namespace UserService.Contracts.Infrastructure
{
  public interface IPaymentsClient : IScopedDependency
  {
    [Post("/payments")]
    Task<PaymentResponseDto> ProcessPaymentAsync([Body] PaymentRequestDto request);
    

    [Get("/payments/{id}")]
    Task<PaymentResponseDto> GetPaymentStatusAsync(string id);
  }


}


