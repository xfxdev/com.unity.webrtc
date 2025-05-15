#pragma once

#include <api/video_codecs/sdp_video_format.h>
#include <api/video_codecs/video_decoder_factory.h>

namespace unity
{
namespace webrtc
{
    using namespace ::webrtc;

    class IGraphicsDevice;
    class ProfilerMarkerFactory;
    class UnityVideoDecoderFactory : public VideoDecoderFactory
    {
    public:
        virtual std::vector<webrtc::SdpVideoFormat> GetSupportedFormats() const override;

        virtual std::unique_ptr<VideoDecoder> Create(const Environment& env, const SdpVideoFormat& format) override;

        UnityVideoDecoderFactory(IGraphicsDevice* gfxDevice, ProfilerMarkerFactory* profiler);
        ~UnityVideoDecoderFactory() override;

    private:
        ProfilerMarkerFactory* profiler_;
        std::map<std::string, std::unique_ptr<VideoDecoderFactory>> factories_;
    };
} // namespace webrtc
} // namespace unity
