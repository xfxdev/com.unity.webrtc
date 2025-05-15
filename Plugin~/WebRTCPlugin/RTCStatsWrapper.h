#pragma once

#include <stdint.h>

namespace webrtc
{
class RTCStats;
class RTCCertificateStats;
class RTCCodecStats;
class RTCDataChannelStats;
class RTCIceCandidatePairStats;
class RTCIceCandidateStats;
class RTCLocalIceCandidateStats;
class RTCRemoteIceCandidateStats;
class RTCPeerConnectionStats;
class RTCRtpStreamStats;
class RTCReceivedRtpStreamStats;
class RTCSentRtpStreamStats;
class RTCInboundRtpStreamStats;
class RTCOutboundRtpStreamStats;
class RTCRemoteInboundRtpStreamStats;
class RTCRemoteOutboundRtpStreamStats;
class RTCMediaSourceStats;
class RTCAudioSourceStats;
class RTCVideoSourceStats;
class RTCTransportStats;
class RTCAudioPlayoutStats;
} // namespace webrtc

namespace unity
{
namespace webrtc
{

    class RTCStats_C
    {
    public:
        const char* id;
        int64_t timestamp;
        explicit RTCStats_C(const ::webrtc::RTCStats* stats);
    };

    class RTCCertificateStats_C
    {
    public:
        RTCStats_C rtc_stats;
        const char* fingerprint;
        const char* fingerprint_algorithm;
        const char* base64_certificate;
        const char* issuer_certificate_id;
        explicit RTCCertificateStats_C(::webrtc::RTCCertificateStats* stats);
    };

    class RTCCodecStats_C
    {
    public:
        RTCStats_C rtc_stats;
        const char* transport_id;
        uint32_t payload_type;
        const char* mime_type;
        uint32_t clock_rate;
        uint32_t channels;
        const char* sdp_fmtp_line;
        explicit RTCCodecStats_C(::webrtc::RTCCodecStats* stats);
    };

    class RTCDataChannelStats_C
    {
    public:
        RTCStats_C rtc_stats;
        const char* label;
        const char* protocol;
        int32_t data_channel_identifier;
        const char* state;
        uint32_t messages_sent;
        uint64_t bytes_sent;
        uint32_t messages_received;
        uint64_t bytes_received;
        explicit RTCDataChannelStats_C(::webrtc::RTCDataChannelStats* stats);
    };

    class RTCIceCandidatePairStats_C
    {
    public:
        RTCStats_C rtc_stats;
        const char* transport_id;
        const char* local_candidate_id;
        const char* remote_candidate_id;
        const char* state;
        uint64_t priority;
        bool nominated;
        bool writable;
        uint64_t packets_sent;
        uint64_t packets_received;
        uint64_t bytes_sent;
        uint64_t bytes_received;
        double total_round_trip_time;
        double current_round_trip_time;
        double available_outgoing_bitrate;
        double available_incoming_bitrate;
        uint64_t requests_received;
        uint64_t requests_sent;
        uint64_t responses_received;
        uint64_t responses_sent;
        uint64_t consent_requests_sent;
        uint64_t packets_discarded_on_send;
        uint64_t bytes_discarded_on_send;
        double last_packet_received_timestamp;
        double last_packet_sent_timestamp;
        explicit RTCIceCandidatePairStats_C(::webrtc::RTCIceCandidatePairStats* stats);
    };

    class RTCIceCandidateStats_C
    {
    public:
        RTCStats_C rtc_stats;
        const char* transport_id;
        bool is_remote; // Obsolete
        const char* network_type;
        const char* ip;
        const char* address;
        int32_t port;
        const char* protocol;
        const char* relay_protocol;
        const char* candidate_type;
        int32_t priority;
        const char* url;
        const char* foundation;
        const char* related_address;
        int32_t related_port;
        const char* username_fragment;
        const char* tcp_type;
        bool vpn;
        const char* network_adapter_type;
        explicit RTCIceCandidateStats_C(::webrtc::RTCIceCandidateStats* stats);
    };

    class RTCPeerConnectionStats_C
    {
    public:
        RTCStats_C rtc_stats;
        uint32_t data_channels_opened;
        uint32_t data_channels_closed;
        explicit RTCPeerConnectionStats_C(::webrtc::RTCPeerConnectionStats* stats);
    };

    class RTCRtpStreamStats_C
    {
    public:
        RTCStats_C rtc_stats;
        uint32_t ssrc;
        const char* kind;
        const char* transport_id;
        const char* codec_id;
        explicit RTCRtpStreamStats_C(::webrtc::RTCRtpStreamStats* stats);
    };

    class RTCReceivedRtpStreamStats_C
    {
    public:
        RTCRtpStreamStats_C base;
        double jitter;
        int32_t packets_lost;
        explicit RTCReceivedRtpStreamStats_C(::webrtc::RTCReceivedRtpStreamStats* stats);
    };

    class RTCSentRtpStreamStats_C
    {
    public:
        RTCRtpStreamStats_C base;
        uint64_t packets_sent;
        uint64_t bytes_sent;
        explicit RTCSentRtpStreamStats_C(::webrtc::RTCSentRtpStreamStats* stats);
    };

    class RTCInboundRtpStreamStats_C
    {
    public:
        RTCReceivedRtpStreamStats_C base;
        const char* playout_id;
        const char* track_identifier;
        const char* mid;
        const char* remote_id;
        uint32_t packets_received;
        uint64_t packets_discarded;
        uint64_t fec_packets_received;
        uint64_t fec_bytes_received;
        uint64_t fec_packets_discarded;
        uint32_t fec_ssrc;
        uint64_t bytes_received;
        uint64_t header_bytes_received;
        uint64_t retransmitted_packets_received;
        uint64_t retransmitted_bytes_received;
        uint32_t rtx_ssrc;
        double last_packet_received_timestamp;
        double jitter_buffer_delay;
        double jitter_buffer_target_delay;
        double jitter_buffer_minimum_delay;
        uint64_t jitter_buffer_emitted_count;
        uint64_t total_samples_received;
        uint64_t concealed_samples;
        uint64_t silent_concealed_samples;
        uint64_t concealment_events;
        uint64_t inserted_samples_for_deceleration;
        uint64_t removed_samples_for_acceleration;
        double audio_level;
        double total_audio_energy;
        double total_samples_duration;
        uint32_t frames_received;
        uint32_t frame_width;
        uint32_t frame_height;
        double frames_per_second;
        uint32_t frames_decoded;
        uint32_t key_frames_decoded;
        uint32_t frames_dropped;
        double total_decode_time;
        double total_processing_delay;
        double total_assembly_time;
        uint32_t frames_assembled_from_multiple_packets;
        double total_inter_frame_delay;
        double total_squared_inter_frame_delay;
        uint32_t pause_count;
        double total_pauses_duration;
        uint32_t freeze_count;
        double total_freezes_duration;
        const char* content_type;
        double estimated_playout_timestamp;
        const char* decoder_implementation;
        uint32_t fir_count;
        uint32_t pli_count;
        uint32_t nack_count;
        uint64_t qp_sum;
        double total_corruption_probability;
        double total_squared_corruption_probability;
        uint64_t corruption_measurements;
        const char* goog_timing_frame_info;
        bool power_efficient_decoder;
        uint64_t jitter_buffer_flushes;
        uint64_t delayed_packet_outage_samples;
        double relative_packet_arrival_delay;
        uint32_t interruption_count;
        double total_interruption_duration;
        double min_playout_delay;
        explicit RTCInboundRtpStreamStats_C(::webrtc::RTCInboundRtpStreamStats* stats);
    };

    class RTCOutboundRtpStreamStats_C
    {
    public:
        RTCSentRtpStreamStats_C base;
        const char* media_source_id;
        const char* remote_id;
        const char* mid;
        const char* rid;
        uint32_t encoding_index;
        uint64_t retransmitted_packets_sent;
        uint64_t header_bytes_sent;
        uint64_t retransmitted_bytes_sent;
        double target_bitrate;
        uint32_t frames_encoded;
        uint32_t key_frames_encoded;
        double total_encode_time;
        uint64_t total_encoded_bytes_target;
        uint32_t frame_width;
        uint32_t frame_height;
        double frames_per_second;
        uint32_t frames_sent;
        uint32_t huge_frames_sent;
        double total_packet_send_delay;
        const char* quality_limitation_reason;
        const char* quality_limitation_durations; // std::map<std::string, double>
        uint32_t quality_limitation_resolution_changes;
        const char* content_type;
        const char* encoder_implementation;
        uint32_t fir_count;
        uint32_t pli_count;
        uint32_t nack_count;
        uint64_t qp_sum;
        bool active;
        bool power_efficient_encoder;
        const char* scalability_mode;
        uint32_t rtx_ssrc;
        explicit RTCOutboundRtpStreamStats_C(::webrtc::RTCOutboundRtpStreamStats* stats);
        ~RTCOutboundRtpStreamStats_C();
    };

    class RTCRemoteInboundRtpStreamStats_C
    {
    public:
        RTCReceivedRtpStreamStats_C base;
        const char* local_id;
        double round_trip_time;
        double fraction_lost;
        double total_round_trip_time;
        int32_t round_trip_time_measurements;
        explicit RTCRemoteInboundRtpStreamStats_C(::webrtc::RTCRemoteInboundRtpStreamStats* stats);
    };

    class RTCRemoteOutboundRtpStreamStats_C
    {
    public:
        RTCSentRtpStreamStats_C base;
        const char* local_id;
        double remote_timestamp;
        uint64_t reports_sent;
        double round_trip_time;
        uint64_t round_trip_time_measurements;
        double total_round_trip_time;
        explicit RTCRemoteOutboundRtpStreamStats_C(::webrtc::RTCRemoteOutboundRtpStreamStats* stats);
    };

    class RTCMediaSourceStats_C
    {
    public:
        RTCStats_C rtc_stats;
        const char* track_identifier;
        const char* kind;

        // audio
        double audio_level;
        double total_audio_energy;
        double total_samples_duration;
        double echo_return_loss;
        double echo_return_loss_enhancement;

        // video
        uint32_t width;
        uint32_t height;
        uint32_t frames;
        double frames_per_second;

        explicit RTCMediaSourceStats_C(::webrtc::RTCMediaSourceStats* stats);
    };

    class RTCTransportStats_C
    {
    public:
        RTCStats_C rtc_stats;
        uint64_t bytes_sent;
        uint64_t packets_sent;
        uint64_t bytes_received;
        uint64_t packets_received;
        const char* rtcp_transport_stats_id;
        const char* dtls_state;
        const char* selected_candidate_pair_id;
        const char* local_certificate_id;
        const char* remote_certificate_id;
        const char* tls_version;
        const char* dtls_cipher;
        const char* dtls_role;
        const char* srtp_cipher;
        uint32_t selected_candidate_pair_changes;
        const char* ice_role;
        const char* ice_local_username_fragment;
        const char* ice_state;
        explicit RTCTransportStats_C(::webrtc::RTCTransportStats* stats);
    };

    class RTCAudioPlayoutStats_C
    {
    public:
        RTCStats_C rtc_stats;
        const char* kind;
        double synthesized_samples_duration;
        uint64_t synthesized_samples_events;
        double total_samples_duration;
        double total_playout_delay;
        uint64_t total_samples_count;
        explicit RTCAudioPlayoutStats_C(::webrtc::RTCAudioPlayoutStats* stats);
    };

    enum RTCStatsType
    {
        Codec = 0,
        InboundRtp = 1,
        OutboundRtp = 2,
        RemoteInboundRtp = 3,
        RemoteOutboundRtp = 4,
        MediaSource = 5,
        MediaPlayOut = 6,
        PeerConnection = 7,
        DataChannel = 8,
        Transport = 9,
        CandidatePair = 10,
        LocalCandidate = 11,
        RemoteCandidate = 12,
        Certificate = 13,
        Unknown = 999,
    };

    class RTCStatsWrapperFactory
    {
    public:
        static RTCStatsType MapRTCStatsType(const ::webrtc::RTCStats* stat);
        static void* Wrap(const ::webrtc::RTCStats* stat);
        static void Destroy(void* wrapper, RTCStatsType type);
    };

} // namespace webrtc
} // namespace unity
