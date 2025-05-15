#include "Utils.h"

#include "pch.h"

namespace unity {
namespace webrtc {

char* Utils::ConvertString(const std::string& str) {
    if (str.empty()) {
        return nullptr;
    }
    const size_t size = str.size();
    char* ret = static_cast<char*>(CoTaskMemAlloc(size + sizeof(char)));
    str.copy(ret, size);
    ret[size] = '\0';
    return ret;
}

char* Utils::ConvertString(std::string_view str) {
    if (str.empty()) {
        return nullptr;
    }
    const size_t size = str.size();
    char* ret = static_cast<char*>(CoTaskMemAlloc(size + sizeof(char)));
    str.copy(ret, size);
    ret[size] = '\0';
    return ret;
}

char* Utils::ConvertString(const std::optional<std::string>& str) {
    if (!str.has_value()) {
        return nullptr;
    }
    return Utils::ConvertString(str.value());
}

}  // namespace webrtc
}  // namespace unity
