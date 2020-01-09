
#include "integration.h"

#ifdef _WIN32
#include <regex>
#else
#include <regex>
#include <re2/re2.h>
#endif
#include <sstream>

#include "util.h"
#include "logging.h"

namespace trace {

AssemblyReference::AssemblyReference(const WSTRING& str)
    : name(GetNameFromAssemblyReferenceString(str)),
      version(GetVersionFromAssemblyReferenceString(str)),
      locale(GetLocaleFromAssemblyReferenceString(str)),
      public_key(GetPublicKeyFromAssemblyReferenceString(str)) {}

namespace {

WSTRING GetNameFromAssemblyReferenceString(const WSTRING& wstr) {
  WSTRING name = wstr;

  auto pos = name.find(','_W);
  if (pos != WSTRING::npos) {
    name = name.substr(0, pos);
  }

  // strip spaces
  pos = name.rfind(' '_W);
  if (pos != WSTRING::npos) {
    name = name.substr(0, pos);
  }

  return name;
}

Version GetVersionFromAssemblyReferenceString(const WSTRING& str) {
  unsigned short major = 0;
  unsigned short minor = 0;
  unsigned short build = 0;
  unsigned short revision = 0;

  std::string cross_os_string = ToString(str);
  std::size_t current_sz;
  std::vector<int> vector_match;
  std::size_t pos_version_start = cross_os_string.find("Version=");
  if (pos_version_start != std::string::npos) {
    pos_version_start += 8;
    std::size_t pos_version_end = cross_os_string.find(",", pos_version_start);
    std::string substring;
    
    if (pos_version_end == std::string::npos) {
      substring = cross_os_string.substr(pos_version_start);
    } else {
      substring = cross_os_string.substr(pos_version_start, pos_version_end - pos_version_start);
    }

    size_t next_position;
    for (int i = 0; i < 4; i++) {
      try {
        vector_match.push_back(std::stoi(substring, &next_position));

        if (next_position == std::string::npos || next_position == substring.length() || substring[next_position] != '.'_W) {
          break;
        }
        substring = substring.substr(next_position + 1);
      } catch (const std::exception& ex) {
        throw ex;
      }
    }

    if (vector_match.size() == 4) {
      major = vector_match[0];
      minor = vector_match[1];
      build = vector_match[2];
      revision = vector_match[3];
    }
  }

  return {major, minor, build, revision};
}

WSTRING GetLocaleFromAssemblyReferenceString(const WSTRING& str) {
  WSTRING locale = "neutral"_W;

  std::string cross_os_string = ToString(str);
  std::size_t pos_culture_start = cross_os_string.find("Culture=");
  if (pos_culture_start != std::string::npos) {
    pos_culture_start += 8;
    std::size_t pos_culture_end = cross_os_string.find_first_not_of("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", pos_culture_start);

    if (pos_culture_end == std::string::npos) {
      locale = ToWSTRING(cross_os_string.substr(pos_culture_start));
    } else {
      locale = ToWSTRING(cross_os_string.substr(pos_culture_start, pos_culture_end - pos_culture_start));
    }
  }

  return locale;
}

PublicKey GetPublicKeyFromAssemblyReferenceString(const WSTRING& str) {
  BYTE data[8] = {0};

  std::string token_string;
  std::string cross_os_string = ToString(str);
  std::size_t pos_token_start = cross_os_string.find("PublicKeyToken=");
  if (pos_token_start != std::string::npos) {
    pos_token_start += 15;
    std::size_t pos_token_end = cross_os_string.find_first_not_of("abcdefABCDEF0123456789");

    if (pos_token_end == std::string::npos) {
      token_string = cross_os_string.substr(pos_token_start);
    } else {
      token_string = cross_os_string.substr(pos_token_start, pos_token_end - pos_token_start);
    }

    if (token_string.length() == 16) {
      for (int i = 0; i < 8; i++) {
        auto s = token_string.substr(i * 2, 2);
        unsigned long x;
        std::stringstream(s) >> std::hex >> x;
        data[i] = BYTE(x);
      }
    }
  }

  return PublicKey(data);
}

}  // namespace

}  // namespace trace
