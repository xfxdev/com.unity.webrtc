#include <optional>
#include <string>

namespace unity {
namespace webrtc {

class Utils {
public:
    static char* ConvertString(const std::string& str);
    static char* ConvertString(std::string_view str);
    static char* ConvertString(const std::optional<std::string>& str);
};

}  // namespace webrtc
}  // namespace unity
