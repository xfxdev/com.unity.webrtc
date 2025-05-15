#include "RTCStatsWrapper.h"

#include <api/stats/rtcstats_objects.h>

#include "Utils.h"
#include "pch.h"

namespace unity
{
namespace webrtc
{

// String from optional<string>
#define SET_STR_FIELD(name) name(stats->name.has_value() ? stats->name->c_str() : nullptr)

// Numeric from optional<T> (int, float, double, etc.)
#define SET_NUM_FIELD(name) name(stats->name.value_or(0))

// Boolean from optional<bool>
#define SET_BOOL_FIELD(name) name(stats->name.value_or(false))

    template<typename T>
    std::string map_to_string(const std::map<std::string, T>& m)
    {
        static_assert(std::is_arithmetic<T>::value, "Only numeric types supported");

        if (m.empty())
        {
            return ""; // Early return if the map is empty
        }

        std::string result;
        result.reserve(m.size() * 24);

        for (auto iter = m.cbegin(); iter != m.cend(); ++iter)
        {
            result.append(iter->first);
            result.push_back('=');
            result.append(std::to_string(iter->second));

            if (std::next(iter) != m.cend())
            {
                result += ';';
            }
        }

        return result;
    }

    // ---- RTCStats_C ----
    RTCStats_C::RTCStats_C(const ::webrtc::RTCStats* stats)
        : id(stats->id().c_str())
        , timestamp(stats->timestamp().us_or(0))
    {
    }

    // ---- RTCCertificateStats_C ----
    RTCCertificateStats_C::RTCCertificateStats_C(::webrtc::RTCCertificateStats* stats)
        : rtc_stats(stats)
        , SET_STR_FIELD(fingerprint)
        , SET_STR_FIELD(fingerprint_algorithm)
        , SET_STR_FIELD(base64_certificate)
        , SET_STR_FIELD(issuer_certificate_id)
    {
    }

    // ---- RTCCodecStats_C ----
    RTCCodecStats_C::RTCCodecStats_C(::webrtc::RTCCodecStats* stats)
        : rtc_stats(stats)
        , SET_STR_FIELD(transport_id)
        , SET_NUM_FIELD(payload_type)
        , SET_STR_FIELD(mime_type)
        , SET_NUM_FIELD(clock_rate)
        , SET_NUM_FIELD(channels)
        , SET_STR_FIELD(sdp_fmtp_line)
    {
    }

    // ---- RTCDataChannelStats_C ----
    RTCDataChannelStats_C::RTCDataChannelStats_C(::webrtc::RTCDataChannelStats* stats)
        : rtc_stats(stats)
        , SET_STR_FIELD(label)
        , SET_STR_FIELD(protocol)
        , SET_NUM_FIELD(data_channel_identifier)
        , SET_STR_FIELD(state)
        , SET_NUM_FIELD(messages_sent)
        , SET_NUM_FIELD(bytes_sent)
        , SET_NUM_FIELD(messages_received)
        , SET_NUM_FIELD(bytes_received)
    {
    }

    // ---- RTCIceCandidatePairStats_C ----
    RTCIceCandidatePairStats_C::RTCIceCandidatePairStats_C(::webrtc::RTCIceCandidatePairStats* stats)
        : rtc_stats(stats)
        , SET_STR_FIELD(transport_id)
        , SET_STR_FIELD(local_candidate_id)
        , SET_STR_FIELD(remote_candidate_id)
        , SET_STR_FIELD(state)
        , SET_NUM_FIELD(priority)
        , SET_BOOL_FIELD(nominated)
        , SET_BOOL_FIELD(writable)
        , SET_NUM_FIELD(packets_sent)
        , SET_NUM_FIELD(packets_received)
        , SET_NUM_FIELD(bytes_sent)
        , SET_NUM_FIELD(bytes_received)
        , SET_NUM_FIELD(total_round_trip_time)
        , SET_NUM_FIELD(current_round_trip_time)
        , SET_NUM_FIELD(available_outgoing_bitrate)
        , SET_NUM_FIELD(available_incoming_bitrate)
        , SET_NUM_FIELD(requests_received)
        , SET_NUM_FIELD(requests_sent)
        , SET_NUM_FIELD(responses_received)
        , SET_NUM_FIELD(responses_sent)
        , SET_NUM_FIELD(consent_requests_sent)
        , SET_NUM_FIELD(packets_discarded_on_send)
        , SET_NUM_FIELD(bytes_discarded_on_send)
        , SET_NUM_FIELD(last_packet_received_timestamp)
        , SET_NUM_FIELD(last_packet_sent_timestamp)
    {
    }

    // ---- RTCIceCandidateStats_C ----
    RTCIceCandidateStats_C::RTCIceCandidateStats_C(::webrtc::RTCIceCandidateStats* stats)
        : rtc_stats(stats)
        , SET_STR_FIELD(transport_id)
        , SET_BOOL_FIELD(is_remote)
        , SET_STR_FIELD(network_type)
        , SET_STR_FIELD(ip)
        , SET_STR_FIELD(address)
        , SET_NUM_FIELD(port)
        , SET_STR_FIELD(protocol)
        , SET_STR_FIELD(relay_protocol)
        , SET_STR_FIELD(candidate_type)
        , SET_NUM_FIELD(priority)
        , SET_STR_FIELD(url)
        , SET_STR_FIELD(foundation)
        , SET_STR_FIELD(related_address)
        , SET_NUM_FIELD(related_port)
        , SET_STR_FIELD(username_fragment)
        , SET_STR_FIELD(tcp_type)
        , SET_BOOL_FIELD(vpn)
        , SET_STR_FIELD(network_adapter_type)
    {
    }

    // ---- RTCPeerConnectionStats_C ----
    RTCPeerConnectionStats_C::RTCPeerConnectionStats_C(::webrtc::RTCPeerConnectionStats* stats)
        : rtc_stats(stats)
        , SET_NUM_FIELD(data_channels_opened)
        , SET_NUM_FIELD(data_channels_closed)
    {
    }

    // ---- RTCRtpStreamStats_C ----
    RTCRtpStreamStats_C::RTCRtpStreamStats_C(::webrtc::RTCRtpStreamStats* stats)
        : rtc_stats(stats)
        , SET_NUM_FIELD(ssrc)
        , SET_STR_FIELD(kind)
        , SET_STR_FIELD(transport_id)
        , SET_STR_FIELD(codec_id)
    {
    }

    // ---- RTCReceivedRtpStreamStats_C ----
    RTCReceivedRtpStreamStats_C::RTCReceivedRtpStreamStats_C(::webrtc::RTCReceivedRtpStreamStats* stats)
        : base(stats)
        , SET_NUM_FIELD(jitter)
        , SET_NUM_FIELD(packets_lost)
    {
    }

    // ---- RTCSentRtpStreamStats_C ----
    RTCSentRtpStreamStats_C::RTCSentRtpStreamStats_C(::webrtc::RTCSentRtpStreamStats* stats)
        : base(stats)
        , SET_NUM_FIELD(packets_sent)
        , SET_NUM_FIELD(bytes_sent)
    {
    }

    // ---- RTCInboundRtpStreamStats_C ----
    RTCInboundRtpStreamStats_C::RTCInboundRtpStreamStats_C(::webrtc::RTCInboundRtpStreamStats* stats)
        : base(stats)
        , SET_STR_FIELD(playout_id)
        , SET_STR_FIELD(track_identifier)
        , SET_STR_FIELD(mid)
        , SET_STR_FIELD(remote_id)
        , SET_NUM_FIELD(packets_received)
        , SET_NUM_FIELD(packets_discarded)
        , SET_NUM_FIELD(fec_packets_received)
        , SET_NUM_FIELD(fec_bytes_received)
        , SET_NUM_FIELD(fec_packets_discarded)
        , SET_NUM_FIELD(fec_ssrc)
        , SET_NUM_FIELD(bytes_received)
        , SET_NUM_FIELD(header_bytes_received)
        , SET_NUM_FIELD(retransmitted_packets_received)
        , SET_NUM_FIELD(retransmitted_bytes_received)
        , SET_NUM_FIELD(rtx_ssrc)
        , SET_NUM_FIELD(last_packet_received_timestamp)
        , SET_NUM_FIELD(jitter_buffer_delay)
        , SET_NUM_FIELD(jitter_buffer_target_delay)
        , SET_NUM_FIELD(jitter_buffer_minimum_delay)
        , SET_NUM_FIELD(jitter_buffer_emitted_count)
        , SET_NUM_FIELD(total_samples_received)
        , SET_NUM_FIELD(concealed_samples)
        , SET_NUM_FIELD(silent_concealed_samples)
        , SET_NUM_FIELD(concealment_events)
        , SET_NUM_FIELD(inserted_samples_for_deceleration)
        , SET_NUM_FIELD(removed_samples_for_acceleration)
        , SET_NUM_FIELD(audio_level)
        , SET_NUM_FIELD(total_audio_energy)
        , SET_NUM_FIELD(total_samples_duration)
        , SET_NUM_FIELD(frames_received)
        , SET_NUM_FIELD(frame_width)
        , SET_NUM_FIELD(frame_height)
        , SET_NUM_FIELD(frames_per_second)
        , SET_NUM_FIELD(frames_decoded)
        , SET_NUM_FIELD(key_frames_decoded)
        , SET_NUM_FIELD(frames_dropped)
        , SET_NUM_FIELD(total_decode_time)
        , SET_NUM_FIELD(total_processing_delay)
        , SET_NUM_FIELD(total_assembly_time)
        , SET_NUM_FIELD(frames_assembled_from_multiple_packets)
        , SET_NUM_FIELD(total_inter_frame_delay)
        , SET_NUM_FIELD(total_squared_inter_frame_delay)
        , SET_NUM_FIELD(pause_count)
        , SET_NUM_FIELD(total_pauses_duration)
        , SET_NUM_FIELD(freeze_count)
        , SET_NUM_FIELD(total_freezes_duration)
        , SET_STR_FIELD(content_type)
        , SET_NUM_FIELD(estimated_playout_timestamp)
        , SET_STR_FIELD(decoder_implementation)
        , SET_NUM_FIELD(fir_count)
        , SET_NUM_FIELD(pli_count)
        , SET_NUM_FIELD(nack_count)
        , SET_NUM_FIELD(qp_sum)
        , SET_NUM_FIELD(total_corruption_probability)
        , SET_NUM_FIELD(total_squared_corruption_probability)
        , SET_NUM_FIELD(corruption_measurements)
        , SET_STR_FIELD(goog_timing_frame_info)
        , SET_BOOL_FIELD(power_efficient_decoder)
        , SET_NUM_FIELD(jitter_buffer_flushes)
        , SET_NUM_FIELD(delayed_packet_outage_samples)
        , SET_NUM_FIELD(relative_packet_arrival_delay)
        , SET_NUM_FIELD(interruption_count)
        , SET_NUM_FIELD(total_interruption_duration)
        , SET_NUM_FIELD(min_playout_delay)
    {
    }

    // ---- RTCOutboundRtpStreamStats_C ----
    RTCOutboundRtpStreamStats_C::RTCOutboundRtpStreamStats_C(::webrtc::RTCOutboundRtpStreamStats* stats)
        : base(stats)
        , SET_STR_FIELD(media_source_id)
        , SET_STR_FIELD(remote_id)
        , SET_STR_FIELD(mid)
        , SET_STR_FIELD(rid)
        , SET_NUM_FIELD(encoding_index)
        , SET_NUM_FIELD(retransmitted_packets_sent)
        , SET_NUM_FIELD(header_bytes_sent)
        , SET_NUM_FIELD(retransmitted_bytes_sent)
        , SET_NUM_FIELD(target_bitrate)
        , SET_NUM_FIELD(frames_encoded)
        , SET_NUM_FIELD(key_frames_encoded)
        , SET_NUM_FIELD(total_encode_time)
        , SET_NUM_FIELD(total_encoded_bytes_target)
        , SET_NUM_FIELD(frame_width)
        , SET_NUM_FIELD(frame_height)
        , SET_NUM_FIELD(frames_per_second)
        , SET_NUM_FIELD(frames_sent)
        , SET_NUM_FIELD(huge_frames_sent)
        , SET_NUM_FIELD(total_packet_send_delay)
        , SET_STR_FIELD(quality_limitation_reason)
        , SET_NUM_FIELD(quality_limitation_resolution_changes)
        , SET_STR_FIELD(content_type)
        , SET_STR_FIELD(encoder_implementation)
        , SET_NUM_FIELD(fir_count)
        , SET_NUM_FIELD(pli_count)
        , SET_NUM_FIELD(nack_count)
        , SET_NUM_FIELD(qp_sum)
        , SET_BOOL_FIELD(active)
        , SET_BOOL_FIELD(power_efficient_encoder)
        , SET_STR_FIELD(scalability_mode)
        , SET_NUM_FIELD(rtx_ssrc)
    {
        // convert map to string
        if (stats->quality_limitation_durations.has_value())
        {
            std::string ret(map_to_string(stats->quality_limitation_durations.value()));
            quality_limitation_durations = Utils::ConvertString(ret);
        }
    }
    RTCOutboundRtpStreamStats_C::~RTCOutboundRtpStreamStats_C() { SAFE_COTASKMEMFREE(quality_limitation_durations); }

    // ---- RTCRemoteInboundRtpStreamStats_C ----
    RTCRemoteInboundRtpStreamStats_C::RTCRemoteInboundRtpStreamStats_C(::webrtc::RTCRemoteInboundRtpStreamStats* stats)
        : base(stats)
        , SET_STR_FIELD(local_id)
        , SET_NUM_FIELD(round_trip_time)
        , SET_NUM_FIELD(fraction_lost)
        , SET_NUM_FIELD(total_round_trip_time)
        , SET_NUM_FIELD(round_trip_time_measurements)
    {
    }

    // ---- RTCRemoteOutboundRtpStreamStats_C ----
    RTCRemoteOutboundRtpStreamStats_C::RTCRemoteOutboundRtpStreamStats_C(
        ::webrtc::RTCRemoteOutboundRtpStreamStats* stats)
        : base(stats)
        , SET_STR_FIELD(local_id)
        , SET_NUM_FIELD(remote_timestamp)
        , SET_NUM_FIELD(reports_sent)
        , SET_NUM_FIELD(round_trip_time)
        , SET_NUM_FIELD(round_trip_time_measurements)
        , SET_NUM_FIELD(total_round_trip_time)
    {
    }

    // ---- RTCMediaSourceStats_C ----
    RTCMediaSourceStats_C::RTCMediaSourceStats_C(::webrtc::RTCMediaSourceStats* stats)
        : rtc_stats(stats)
        , SET_STR_FIELD(track_identifier)
        , SET_STR_FIELD(kind)
    {
        // RTCAudioSourceStats::kType and RTCVideoSourceStats::kType both have
        // the value "media-source", but they are distinguishable with pointer
        // equality (==).
        if (stats->type() == ::webrtc::RTCAudioSourceStats::kType)
        {
            const auto& audioStats = stats->cast_to<::webrtc::RTCAudioSourceStats>();
            audio_level = audioStats.audio_level.value_or(0);
            total_audio_energy = audioStats.total_audio_energy.value_or(0);
            total_samples_duration = audioStats.total_samples_duration.value_or(0);
            echo_return_loss = audioStats.echo_return_loss.value_or(0);
            echo_return_loss_enhancement = audioStats.echo_return_loss_enhancement.value_or(0);
        }
        else if (stats->type() == ::webrtc::RTCVideoSourceStats::kType)
        {
            const auto& videoStats = stats->cast_to<::webrtc::RTCVideoSourceStats>();
            width = videoStats.width.value_or(0);
            height = videoStats.height.value_or(0);
            frames = videoStats.frames.value_or(0);
            frames_per_second = videoStats.frames_per_second.value_or(0);
        }
    }

    // ---- RTCTransportStats_C ----
    RTCTransportStats_C::RTCTransportStats_C(::webrtc::RTCTransportStats* stats)
        : rtc_stats(stats)
        , SET_NUM_FIELD(bytes_sent)
        , SET_NUM_FIELD(packets_sent)
        , SET_NUM_FIELD(bytes_received)
        , SET_NUM_FIELD(packets_received)
        , SET_STR_FIELD(rtcp_transport_stats_id)
        , SET_STR_FIELD(dtls_state)
        , SET_STR_FIELD(selected_candidate_pair_id)
        , SET_STR_FIELD(local_certificate_id)
        , SET_STR_FIELD(remote_certificate_id)
        , SET_STR_FIELD(tls_version)
        , SET_STR_FIELD(dtls_cipher)
        , SET_STR_FIELD(dtls_role)
        , SET_STR_FIELD(srtp_cipher)
        , SET_NUM_FIELD(selected_candidate_pair_changes)
        , SET_STR_FIELD(ice_role)
        , SET_STR_FIELD(ice_local_username_fragment)
        , SET_STR_FIELD(ice_state)
    {
    }

    // ---- RTCAudioPlayoutStats_C ----
    RTCAudioPlayoutStats_C::RTCAudioPlayoutStats_C(::webrtc::RTCAudioPlayoutStats* stats)
        : rtc_stats(stats)
        , SET_STR_FIELD(kind)
        , SET_NUM_FIELD(synthesized_samples_duration)
        , SET_NUM_FIELD(synthesized_samples_events)
        , SET_NUM_FIELD(total_samples_duration)
        , SET_NUM_FIELD(total_playout_delay)
        , SET_NUM_FIELD(total_samples_count)
    {
    }

#undef SET_STR_FIELD
#undef SET_NUM_FIELD
#undef SET_BOOL_FIELD

    // ---- RTCStatsWrapperFactory ----
    RTCStatsType RTCStatsWrapperFactory::MapRTCStatsType(const ::webrtc::RTCStats* stat)
    {
        if (!stat)
        {
            return Unknown;
        }

        static const std::map<const char*, int> mapStatsTypes = {
            { ::webrtc::RTCCertificateStats::kType, RTCStatsType::Certificate }, // "certificate"
            { ::webrtc::RTCCodecStats::kType, RTCStatsType::Codec }, // "codec"
            { ::webrtc::RTCDataChannelStats::kType, RTCStatsType::DataChannel }, // "data-channel"
            { ::webrtc::RTCIceCandidatePairStats::kType, RTCStatsType::CandidatePair }, // "candidate-pair"
            { ::webrtc::RTCLocalIceCandidateStats::kType, RTCStatsType::LocalCandidate }, // "local-candidate"
            { ::webrtc::RTCRemoteIceCandidateStats::kType, RTCStatsType::RemoteCandidate }, // "remote-candidate"
            { ::webrtc::RTCPeerConnectionStats::kType, RTCStatsType::PeerConnection }, // "peer-connection"
            { ::webrtc::RTCInboundRtpStreamStats::kType, RTCStatsType::InboundRtp }, // "inbound-rtp"
            { ::webrtc::RTCOutboundRtpStreamStats::kType, RTCStatsType::OutboundRtp }, // "outbound-rtp"
            { ::webrtc::RTCRemoteInboundRtpStreamStats::kType, RTCStatsType::RemoteInboundRtp }, // "remote-inbound-rtp"
            { ::webrtc::RTCRemoteOutboundRtpStreamStats::kType,
              RTCStatsType::RemoteOutboundRtp }, // "remote-outbound-rtp"
            { ::webrtc::RTCAudioSourceStats::kType, RTCStatsType::MediaSource }, // "media-source"
            { ::webrtc::RTCVideoSourceStats::kType, RTCStatsType::MediaSource }, // "media-source"
            { ::webrtc::RTCTransportStats::kType, RTCStatsType::Transport }, // "transport"
            { ::webrtc::RTCAudioPlayoutStats::kType, RTCStatsType::MediaPlayOut }, // "media-playout"

            // todo: If the following types are deleted from rtcstats_objects.h, delete them as well.
            { "stream", 21 },
            { "track", 22 }
        };

        const auto& iter = mapStatsTypes.find(stat->type());
        if (iter != mapStatsTypes.end())
        {
            return (RTCStatsType)iter->second;
        }
        return Unknown;
    }

    void* RTCStatsWrapperFactory::Wrap(const ::webrtc::RTCStats* stat)
    {
        if (!stat)
        {
            return nullptr;
        }

        const char* type = stat->type();

        /* Note:
            - stat->type() returns a const char*, and kType is also const char*, so comparing pointers via == is valid.
            - This is safe only because WebRTC defines kType as static const char[] and reuses the pointer.
            - This avoids strcmp() overhead.
        */

        if (type == ::webrtc::RTCCertificateStats::kType)
            return new RTCCertificateStats_C((::webrtc::RTCCertificateStats*)stat);
        else if (type == ::webrtc::RTCCodecStats::kType)
            return new RTCCodecStats_C((::webrtc::RTCCodecStats*)stat);
        else if (type == ::webrtc::RTCDataChannelStats::kType)
            return new RTCDataChannelStats_C((::webrtc::RTCDataChannelStats*)stat);
        else if (type == ::webrtc::RTCIceCandidatePairStats::kType)
            return new RTCIceCandidatePairStats_C((::webrtc::RTCIceCandidatePairStats*)stat);
        else if (
            type == ::webrtc::RTCLocalIceCandidateStats::kType || type == ::webrtc::RTCRemoteIceCandidateStats::kType)
            return new RTCIceCandidateStats_C((::webrtc::RTCIceCandidateStats*)stat);
        else if (type == ::webrtc::RTCPeerConnectionStats::kType)
            return new RTCPeerConnectionStats_C((::webrtc::RTCPeerConnectionStats*)stat);
        else if (type == ::webrtc::RTCInboundRtpStreamStats::kType)
            return new RTCInboundRtpStreamStats_C((::webrtc::RTCInboundRtpStreamStats*)stat);
        else if (type == ::webrtc::RTCOutboundRtpStreamStats::kType)
            return new RTCOutboundRtpStreamStats_C((::webrtc::RTCOutboundRtpStreamStats*)stat);
        else if (type == ::webrtc::RTCRemoteInboundRtpStreamStats::kType)
            return new RTCRemoteInboundRtpStreamStats_C((::webrtc::RTCRemoteInboundRtpStreamStats*)stat);
        else if (type == ::webrtc::RTCRemoteOutboundRtpStreamStats::kType)
            return new RTCRemoteOutboundRtpStreamStats_C((::webrtc::RTCRemoteOutboundRtpStreamStats*)stat);
        else if (type == ::webrtc::RTCAudioSourceStats::kType || type == ::webrtc::RTCVideoSourceStats::kType)
            return new RTCMediaSourceStats_C((::webrtc::RTCMediaSourceStats*)stat);
        else if (type == ::webrtc::RTCTransportStats::kType)
            return new RTCTransportStats_C((::webrtc::RTCTransportStats*)stat);
        else if (type == ::webrtc::RTCAudioPlayoutStats::kType)
            return new RTCAudioPlayoutStats_C((::webrtc::RTCAudioPlayoutStats*)stat);

        return nullptr;
    }
    void RTCStatsWrapperFactory::Destroy(void* wrapper, RTCStatsType type)
    {
        if (!wrapper)
        {
            return;
        }

        switch (type)
        {
        case Certificate:
            delete (RTCCertificateStats_C*)wrapper;
            break;
        case Codec:
            delete (RTCCodecStats_C*)wrapper;
            break;
        case DataChannel:
            delete (RTCDataChannelStats_C*)wrapper;
            break;
        case CandidatePair:
            delete (RTCIceCandidatePairStats_C*)wrapper;
            break;
        case LocalCandidate:
        case RemoteCandidate:
            delete (RTCIceCandidateStats_C*)wrapper;
            break;
        case PeerConnection:
            delete (RTCPeerConnectionStats_C*)wrapper;
            break;
        case InboundRtp:
            delete (RTCInboundRtpStreamStats_C*)wrapper;
            break;
        case OutboundRtp:
            delete (RTCOutboundRtpStreamStats_C*)wrapper;
            break;
        case RemoteInboundRtp:
            delete (RTCRemoteInboundRtpStreamStats_C*)wrapper;
            break;
        case RemoteOutboundRtp:
            delete (RTCRemoteOutboundRtpStreamStats_C*)wrapper;
            break;
        case MediaSource:
            delete (RTCMediaSourceStats_C*)wrapper;
            break;
        case Transport:
            delete (RTCTransportStats_C*)wrapper;
            break;
        case MediaPlayOut:
            delete (RTCAudioPlayoutStats_C*)wrapper;
            break;
        default:
            break;
        }
    }
} // namespace webrtc
} // namespace unity
