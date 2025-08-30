using StreamApi.Models;

namespace StreamApi.Services;

public interface IStreamService
{
    IEnumerable<StreamInfoDto> ListStreams();
}
