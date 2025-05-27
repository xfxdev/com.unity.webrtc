using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.WebRTC
{
    /// <summary>
    ///
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        /// <summary>
        ///
        /// </summary>
        public string StringValue { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public enum RTCStatsType
    {
        /// <summary>
        ///
        /// </summary>
        [StringValue("codec")]
        Codec = 0,

        /// <summary>
        ///
        /// </summary>
        [StringValue("inbound-rtp")]
        InboundRtp = 1,

        /// <summary>
        ///
        /// </summary>
        [StringValue("outbound-rtp")]
        OutboundRtp = 2,

        /// <summary>
        ///
        /// </summary>
        [StringValue("remote-inbound-rtp")]
        RemoteInboundRtp = 3,

        /// <summary>
        ///
        /// </summary>
        [StringValue("remote-outbound-rtp")]
        RemoteOutboundRtp = 4,

        /// <summary>
        ///
        /// </summary>
        [StringValue("media-source")]
        MediaSource = 5,

        /// <summary>
        ///
        /// </summary>
        [StringValue("media-playout")]
        MediaPlayOut = 6,

        /// <summary>
        ///
        /// </summary>
        [StringValue("peer-connection")]
        PeerConnection = 7,

        /// <summary>
        ///
        /// </summary>
        [StringValue("data-channel")]
        DataChannel = 8,

        /// <summary>
        ///
        /// </summary>
        [StringValue("transport")]
        Transport = 9,

        /// <summary>
        ///
        /// </summary>
        [StringValue("candidate-pair")]
        CandidatePair = 10,

        /// <summary>
        ///
        /// </summary>
        [StringValue("local-candidate")]
        LocalCandidate = 11,

        /// <summary>
        ///
        /// </summary>
        [StringValue("remote-candidate")]
        RemoteCandidate = 12,

        /// <summary>
        ///
        /// </summary>
        [StringValue("certificate")]
        Certificate = 13,
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCStats
    {
        // internal Dictionary<string, RTCStatsMember> m_members;
        internal Dictionary<string, object> m_dict;

        /// <summary>
        ///
        /// </summary>
        public RTCStatsType Type { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// this timestamp is utc epoch time micro seconds.
        /// </summary>
        public long Timestamp { get; private set; }

        private string json;

        /// <summary>
        ///
        /// </summary>
        public DateTime UtcTimeStamp
        {
            get { return DateTimeOffset.FromUnixTimeMilliseconds(Timestamp / 1000).UtcDateTime; }
        }

        /// <summary>
        ///
        /// </summary>
        public IDictionary<string, object> Dict => m_dict ??= BuildAttributeMap();

        protected virtual Dictionary<string, object> BuildAttributeMap()
        {
            return new Dictionary<string, object>
            {
                // ["id"] = Id,
                // ["timestamp"] = Timestamp
            };
        }

        internal RTCStats(RTCStatsType type, in RTCStatsInternal statsInternal)
        {
            Type = type;
            Id = statsInternal.id;
            Timestamp = statsInternal.timestamp;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            if (json == null)
            {
                IntPtr jsonPtr = WebRTC.Context.StatsToJson(Id);
                if (jsonPtr != IntPtr.Zero)
                {
                    json = jsonPtr.AsAnsiStringWithFreeMem();
                }
            }
            return json;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCCertificateStats : RTCStats
    {
        public string fingerprint { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string fingerprintAlgorithm { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string base64Certificate { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string issuerCertificateId { get; private set; }

        internal RTCCertificateStats(in RTCCertificateStatsInternal statsInternal)
            : base(RTCStatsType.Certificate, statsInternal.rtc_stats)
        {
            fingerprint = statsInternal.fingerprint;
            fingerprintAlgorithm = statsInternal.fingerprint_algorithm;
            base64Certificate = statsInternal.base64_certificate;
            issuerCertificateId = statsInternal.issuer_certificate_id;
        }

        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["fingerprint"] = fingerprint;
            dict["fingerprintAlgorithm"] = fingerprintAlgorithm;
            dict["base64Certificate"] = base64Certificate;
            dict["issuerCertificateId"] = issuerCertificateId;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCCodecStats : RTCStats
    {
        public string transportId { get; private set; }
        public uint payloadType { get; private set; }
        public string mimeType { get; private set; }
        public uint clockRate { get; private set; }
        public uint channels { get; private set; }
        public string sdpFmtpLine { get; private set; }

        internal RTCCodecStats(in RTCCodecStatsInternal statsInternal)
            : base(RTCStatsType.Codec, statsInternal.rtc_stats)
        {
            transportId = statsInternal.transport_id;
            payloadType = statsInternal.payload_type;
            mimeType = statsInternal.mime_type;
            clockRate = statsInternal.clock_rate;
            channels = statsInternal.channels;
            sdpFmtpLine = statsInternal.sdp_fmtp_line;
        }

        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["transportId"] = transportId;
            dict["payloadType"] = payloadType;
            dict["mimeType"] = mimeType;
            dict["clockRate"] = clockRate;
            dict["channels"] = channels;
            dict["sdpFmtpLine"] = sdpFmtpLine;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCDataChannelStats : RTCStats
    {
        public string label { get; private set; }
        public string protocol { get; private set; }
        public int dataChannelIdentifier { get; private set; }
        public string state { get; private set; }
        public uint messagesSent { get; private set; }
        public ulong bytesSent { get; private set; }
        public uint messagesReceived { get; private set; }
        public ulong bytesReceived { get; private set; }

        internal RTCDataChannelStats(in RTCDataChannelStatsInternal statsInternal)
            : base(RTCStatsType.DataChannel, statsInternal.rtc_stats)
        {
            label = statsInternal.label;
            protocol = statsInternal.protocol;
            dataChannelIdentifier = statsInternal.data_channel_identifier;
            state = statsInternal.state;
            messagesSent = statsInternal.messages_sent;
            bytesSent = statsInternal.bytes_sent;
            messagesReceived = statsInternal.messages_received;
            bytesReceived = statsInternal.bytes_received;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["label"] = label;
            dict["protocol"] = protocol;
            dict["dataChannelIdentifier"] = dataChannelIdentifier;
            dict["state"] = state;
            dict["messagesSent"] = messagesSent;
            dict["bytesSent"] = bytesSent;
            dict["messagesReceived"] = messagesReceived;
            dict["bytesReceived"] = bytesReceived;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCIceCandidatePairStats : RTCStats
    {
        public string transportId { get; private set; }
        public string localCandidateId { get; private set; }
        public string remoteCandidateId { get; private set; }
        public string state { get; private set; }
        public ulong priority { get; private set; }
        public bool nominated { get; private set; }
        public bool writable { get; private set; }
        public ulong packetsSent { get; private set; }
        public ulong packetsReceived { get; private set; }
        public ulong bytesSent { get; private set; }
        public ulong bytesReceived { get; private set; }
        public double totalRoundTripTime { get; private set; }
        public double currentRoundTripTime { get; private set; }
        public double availableOutgoingBitrate { get; private set; }
        public double availableIncomingBitrate { get; private set; }
        public ulong requestsReceived { get; private set; }
        public ulong requestsSent { get; private set; }
        public ulong responsesReceived { get; private set; }
        public ulong responsesSent { get; private set; }
        public ulong consentRequestsSent { get; private set; }
        public ulong packetsDiscardedOnSend { get; private set; }
        public ulong bytesDiscardedOnSend { get; private set; }
        public double lastPacketReceivedTimestamp { get; private set; }
        public double lastPacketSentTimestamp { get; private set; }

        internal RTCIceCandidatePairStats(in RTCIceCandidatePairStatsInternal statsInternal)
            : base(RTCStatsType.CandidatePair, statsInternal.rtc_stats)
        {
            transportId = statsInternal.transport_id;
            localCandidateId = statsInternal.local_candidate_id;
            remoteCandidateId = statsInternal.remote_candidate_id;
            state = statsInternal.state;
            priority = statsInternal.priority;
            nominated = statsInternal.nominated;
            writable = statsInternal.writable;
            packetsSent = statsInternal.packets_sent;
            packetsReceived = statsInternal.packets_received;
            bytesSent = statsInternal.bytes_sent;
            bytesReceived = statsInternal.bytes_received;
            totalRoundTripTime = statsInternal.total_round_trip_time;
            currentRoundTripTime = statsInternal.current_round_trip_time;
            availableOutgoingBitrate = statsInternal.available_outgoing_bitrate;
            availableIncomingBitrate = statsInternal.available_incoming_bitrate;
            requestsReceived = statsInternal.requests_received;
            requestsSent = statsInternal.requests_sent;
            responsesReceived = statsInternal.responses_received;
            responsesSent = statsInternal.responses_sent;
            consentRequestsSent = statsInternal.consent_requests_sent;
            packetsDiscardedOnSend = statsInternal.packets_discarded_on_send;
            bytesDiscardedOnSend = statsInternal.bytes_discarded_on_send;
            lastPacketReceivedTimestamp = statsInternal.last_packet_received_timestamp;
            lastPacketSentTimestamp = statsInternal.last_packet_sent_timestamp;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["transportId"] = transportId;
            dict["localCandidateId"] = localCandidateId;
            dict["remoteCandidateId"] = remoteCandidateId;
            dict["state"] = state;
            dict["priority"] = priority;
            dict["nominated"] = nominated;
            dict["writable"] = writable;
            dict["packetsSent"] = packetsSent;
            dict["packetsReceived"] = packetsReceived;
            dict["bytesSent"] = bytesSent;
            dict["bytesReceived"] = bytesReceived;
            dict["totalRoundTripTime"] = totalRoundTripTime;
            dict["currentRoundTripTime"] = currentRoundTripTime;
            dict["availableOutgoingBitrate"] = availableOutgoingBitrate;
            dict["availableIncomingBitrate"] = availableIncomingBitrate;
            dict["requestsReceived"] = requestsReceived;
            dict["requestsSent"] = requestsSent;
            dict["responsesReceived"] = responsesReceived;
            dict["responsesSent"] = responsesSent;
            dict["consentRequestsSent"] = consentRequestsSent;
            dict["packetsDiscardedOnSend"] = packetsDiscardedOnSend;
            dict["bytesDiscardedOnSend"] = bytesDiscardedOnSend;
            dict["lastPacketReceivedTimestamp"] = lastPacketReceivedTimestamp;
            dict["lastPacketSentTimestamp"] = lastPacketSentTimestamp;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCIceCandidateStats : RTCStats
    {
        public string transportId { get; private set; }
        public bool isRemote { get; private set; }
        public string networkType { get; private set; }
        public string ip { get; private set; }
        public string address { get; private set; }
        public int port { get; private set; }
        public string protocol { get; private set; }
        public string relayProtocol { get; private set; }
        public string candidateType { get; private set; }
        public int priority { get; private set; }
        public string url { get; private set; }
        public string foundation { get; private set; }
        public string relatedAddress { get; private set; }
        public int relatedPort { get; private set; }
        public string usernameFragment { get; private set; }
        public string tcpType { get; private set; }
        public bool vpn { get; private set; }
        public string networkAdapterType { get; private set; }

        internal RTCIceCandidateStats(in RTCIceCandidateStatsInternal statsInternal)
            : base(statsInternal.is_remote ? RTCStatsType.RemoteCandidate : RTCStatsType.LocalCandidate, statsInternal.rtc_stats)
        {
            transportId = statsInternal.transport_id;
            isRemote = statsInternal.is_remote;
            networkType = statsInternal.network_type;
            ip = statsInternal.ip;
            address = statsInternal.address;
            port = statsInternal.port;
            protocol = statsInternal.protocol;
            relayProtocol = statsInternal.relay_protocol;
            candidateType = statsInternal.candidate_type;
            priority = statsInternal.priority;
            url = statsInternal.url;
            foundation = statsInternal.foundation;
            relatedAddress = statsInternal.related_address;
            relatedPort = statsInternal.related_port;
            usernameFragment = statsInternal.username_fragment;
            tcpType = statsInternal.tcp_type;
            vpn = statsInternal.vpn;
            networkAdapterType = statsInternal.network_adapter_type;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["transportId"] = transportId;
            dict["isRemote"] = isRemote;
            dict["networkType"] = networkType;
            dict["ip"] = ip;
            dict["address"] = address;
            dict["port"] = port;
            dict["protocol"] = protocol;
            dict["relayProtocol"] = relayProtocol;
            dict["candidateType"] = candidateType;
            dict["priority"] = priority;
            dict["url"] = url;
            dict["foundation"] = foundation;
            dict["relatedAddress"] = relatedAddress;
            dict["relatedPort"] = relatedPort;
            dict["usernameFragment"] = usernameFragment;
            dict["tcpType"] = tcpType;
            dict["vpn"] = vpn;
            dict["networkAdapterType"] = networkAdapterType;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCPeerConnectionStats : RTCStats
    {
        public uint dataChannelsOpened { get; private set; }
        public uint dataChannelsClosed { get; private set; }

        internal RTCPeerConnectionStats(in RTCPeerConnectionStatsInternal statsInternal)
            : base(RTCStatsType.PeerConnection, statsInternal.rtc_stats)
        {
            dataChannelsOpened = statsInternal.data_channels_opened;
            dataChannelsClosed = statsInternal.data_channels_closed;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["dataChannelsOpened"] = dataChannelsOpened;
            dict["dataChannelsClosed"] = dataChannelsClosed;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCRTPStreamStats : RTCStats
    {
        public uint ssrc { get; private set; }
        public string kind { get; private set; }
        public string transportId { get; private set; }
        public string codecId { get; private set; }

        internal RTCRTPStreamStats(RTCStatsType type, in RTCRtpStreamStatsInternal statsInternal)
            : base(type, statsInternal.rtc_stats)
        {
            ssrc = statsInternal.ssrc;
            kind = statsInternal.kind;
            transportId = statsInternal.transport_id;
            codecId = statsInternal.codec_id;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["ssrc"] = ssrc;
            dict["kind"] = kind;
            dict["transportId"] = transportId;
            dict["codecId"] = codecId;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCReceivedRtpStreamStats : RTCRTPStreamStats
    {
        public double jitter { get; private set; }
        public int packetsLost { get; private set; }

        internal RTCReceivedRtpStreamStats(RTCStatsType type, in RTCReceivedRtpStreamStatsInternal statsInternal)
            : base(type, statsInternal.baseStats)
        {
            jitter = statsInternal.jitter;
            packetsLost = statsInternal.packets_lost;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["jitter"] = jitter;
            dict["packetsLost"] = packetsLost;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCSentRtpStreamStats : RTCRTPStreamStats
    {
        public ulong packetsSent { get; private set; }
        public ulong bytesSent { get; private set; }

        internal RTCSentRtpStreamStats(RTCStatsType type, in RTCSentRtpStreamStatsInternal statsInternal)
            : base(type, statsInternal.baseStats)
        {
            packetsSent = statsInternal.packets_sent;
            bytesSent = statsInternal.bytes_sent;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["packetsSent"] = packetsSent;
            dict["bytesSent"] = bytesSent;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCInboundRTPStreamStats : RTCReceivedRtpStreamStats
    {
        public string playoutId { get; private set; }
        public string trackIdentifier { get; private set; }
        public string mid { get; private set; }
        public string remoteId { get; private set; }
        public uint packetsReceived { get; private set; }
        public ulong packetsDiscarded { get; private set; }
        public ulong fecPacketsReceived { get; private set; }
        public ulong fecBytesReceived { get; private set; }
        public ulong fecPacketsDiscarded { get; private set; }
        public uint fecSsrc { get; private set; }
        public ulong bytesReceived { get; private set; }
        public ulong headerBytesReceived { get; private set; }
        public ulong retransmittedPacketsReceived { get; private set; }
        public ulong retransmittedBytesReceived { get; private set; }
        public uint rtxSsrc { get; private set; }
        public double lastPacketReceivedTimestamp { get; private set; }
        public double jitterBufferDelay { get; private set; }
        public double jitterBufferTargetDelay { get; private set; }
        public double jitterBufferMinimumDelay { get; private set; }
        public ulong jitterBufferEmittedCount { get; private set; }
        public ulong totalSamplesReceived { get; private set; }
        public ulong concealedSamples { get; private set; }
        public ulong silentConcealedSamples { get; private set; }
        public ulong concealmentEvents { get; private set; }
        public ulong insertedSamplesForDeceleration { get; private set; }
        public ulong removedSamplesForAcceleration { get; private set; }
        public double audioLevel { get; private set; }
        public double totalAudioEnergy { get; private set; }
        public double totalSamplesDuration { get; private set; }
        public uint framesReceived { get; private set; }
        public uint frameWidth { get; private set; }
        public uint frameHeight { get; private set; }
        public double framesPerSecond { get; private set; }
        public uint framesDecoded { get; private set; }
        public uint keyFramesDecoded { get; private set; }
        public uint framesDropped { get; private set; }
        public double totalDecodeTime { get; private set; }
        public double totalProcessingDelay { get; private set; }
        public double totalAssemblyTime { get; private set; }
        public uint framesAssembledFromMultiplePackets { get; private set; }
        public double totalInterFrameDelay { get; private set; }
        public double totalSquaredInterFrameDelay { get; private set; }
        public uint pauseCount { get; private set; }
        public double totalPausesDuration { get; private set; }
        public uint freezeCount { get; private set; }
        public double totalFreezesDuration { get; private set; }
        public string contentType { get; private set; }
        public double estimatedPlayoutTimestamp { get; private set; }
        public string decoderImplementation { get; private set; }
        public uint firCount { get; private set; }
        public uint pliCount { get; private set; }
        public uint nackCount { get; private set; }
        public ulong qpSum { get; private set; }
        public double totalCorruptionProbability { get; private set; }
        public double totalSquaredCorruptionProbability { get; private set; }
        public ulong corruptionMeasurements { get; private set; }
        public string googTimingFrameInfo { get; private set; }
        public bool powerEfficientDecoder { get; private set; }
        public ulong jitterBufferFlushes { get; private set; }
        public ulong delayedPacketOutageSamples { get; private set; }
        public double relativePacketArrivalDelay { get; private set; }
        public uint interruptionCount { get; private set; }
        public double totalInterruptionDuration { get; private set; }
        public double minPlayoutDelay { get; private set; }

        internal RTCInboundRTPStreamStats(in RTCInboundRtpStreamStatsInternal statsInternal)
            : base(RTCStatsType.InboundRtp, statsInternal.baseStats)
        {
            playoutId = statsInternal.playout_id;
            trackIdentifier = statsInternal.track_identifier;
            mid = statsInternal.mid;
            remoteId = statsInternal.remote_id;
            packetsReceived = statsInternal.packets_received;
            packetsDiscarded = statsInternal.packets_discarded;
            fecPacketsReceived = statsInternal.fec_packets_received;
            fecBytesReceived = statsInternal.fec_bytes_received;
            fecPacketsDiscarded = statsInternal.fec_packets_discarded;
            fecSsrc = statsInternal.fec_ssrc;
            bytesReceived = statsInternal.bytes_received;
            headerBytesReceived = statsInternal.header_bytes_received;
            retransmittedPacketsReceived = statsInternal.retransmitted_packets_received;
            retransmittedBytesReceived = statsInternal.retransmitted_bytes_received;
            rtxSsrc = statsInternal.rtx_ssrc;
            lastPacketReceivedTimestamp = statsInternal.last_packet_received_timestamp;
            jitterBufferDelay = statsInternal.jitter_buffer_delay;
            jitterBufferTargetDelay = statsInternal.jitter_buffer_target_delay;
            jitterBufferMinimumDelay = statsInternal.jitter_buffer_minimum_delay;
            jitterBufferEmittedCount = statsInternal.jitter_buffer_emitted_count;
            totalSamplesReceived = statsInternal.total_samples_received;
            concealedSamples = statsInternal.concealed_samples;
            silentConcealedSamples = statsInternal.silent_concealed_samples;
            concealmentEvents = statsInternal.concealment_events;
            insertedSamplesForDeceleration = statsInternal.inserted_samples_for_deceleration;
            removedSamplesForAcceleration = statsInternal.removed_samples_for_acceleration;
            audioLevel = statsInternal.audio_level;
            totalAudioEnergy = statsInternal.total_audio_energy;
            totalSamplesDuration = statsInternal.total_samples_duration;
            framesReceived = statsInternal.frames_received;
            frameWidth = statsInternal.frame_width;
            frameHeight = statsInternal.frame_height;
            framesPerSecond = statsInternal.frames_per_second;
            framesDecoded = statsInternal.frames_decoded;
            keyFramesDecoded = statsInternal.key_frames_decoded;
            framesDropped = statsInternal.frames_dropped;
            totalDecodeTime = statsInternal.total_decode_time;
            totalProcessingDelay = statsInternal.total_processing_delay;
            totalAssemblyTime = statsInternal.total_assembly_time;
            framesAssembledFromMultiplePackets = statsInternal.frames_assembled_from_multiple_packets;
            totalInterFrameDelay = statsInternal.total_inter_frame_delay;
            totalSquaredInterFrameDelay = statsInternal.total_squared_inter_frame_delay;
            pauseCount = statsInternal.pause_count;
            totalPausesDuration = statsInternal.total_pauses_duration;
            freezeCount = statsInternal.freeze_count;
            totalFreezesDuration = statsInternal.total_freezes_duration;
            contentType = statsInternal.content_type;
            estimatedPlayoutTimestamp = statsInternal.estimated_playout_timestamp;
            decoderImplementation = statsInternal.decoder_implementation;
            firCount = statsInternal.fir_count;
            pliCount = statsInternal.pli_count;
            nackCount = statsInternal.nack_count;
            qpSum = statsInternal.qp_sum;
            totalCorruptionProbability = statsInternal.total_corruption_probability;
            totalSquaredCorruptionProbability = statsInternal.total_squared_corruption_probability;
            corruptionMeasurements = statsInternal.corruption_measurements;
            googTimingFrameInfo = statsInternal.goog_timing_frame_info;
            powerEfficientDecoder = statsInternal.power_efficient_decoder;
            jitterBufferFlushes = statsInternal.jitter_buffer_flushes;
            delayedPacketOutageSamples = statsInternal.delayed_packet_outage_samples;
            relativePacketArrivalDelay = statsInternal.relative_packet_arrival_delay;
            interruptionCount = statsInternal.interruption_count;
            totalInterruptionDuration = statsInternal.total_interruption_duration;
            minPlayoutDelay = statsInternal.min_playout_delay;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["playoutId"] = playoutId;
            dict["trackIdentifier"] = trackIdentifier;
            dict["mid"] = mid;
            dict["remoteId"] = remoteId;
            dict["packetsReceived"] = packetsReceived;
            dict["packetsDiscarded"] = packetsDiscarded;
            dict["fecPacketsReceived"] = fecPacketsReceived;
            dict["fecBytesReceived"] = fecBytesReceived;
            dict["fecPacketsDiscarded"] = fecPacketsDiscarded;
            dict["fecSsrc"] = fecSsrc;
            dict["bytesReceived"] = bytesReceived;
            dict["headerBytesReceived"] = headerBytesReceived;
            dict["retransmittedPacketsReceived"] = retransmittedPacketsReceived;
            dict["retransmittedBytesReceived"] = retransmittedBytesReceived;
            dict["rtxSsrc"] = rtxSsrc;
            dict["lastPacketReceivedTimestamp"] = lastPacketReceivedTimestamp;
            dict["jitterBufferDelay"] = jitterBufferDelay;
            dict["jitterBufferTargetDelay"] = jitterBufferTargetDelay;
            dict["jitterBufferMinimumDelay"] = jitterBufferMinimumDelay;
            dict["jitterBufferEmittedCount"] = jitterBufferEmittedCount;
            dict["totalSamplesReceived"] = totalSamplesReceived;
            dict["concealedSamples"] = concealedSamples;
            dict["silentConcealedSamples"] = silentConcealedSamples;
            dict["concealmentEvents"] = concealmentEvents;
            dict["insertedSamplesForDeceleration"] = insertedSamplesForDeceleration;
            dict["removedSamplesForAcceleration"] = removedSamplesForAcceleration;
            dict["audioLevel"] = audioLevel;
            dict["totalAudioEnergy"] = totalAudioEnergy;
            dict["totalSamplesDuration"] = totalSamplesDuration;
            dict["framesReceived"] = framesReceived;
            dict["frameWidth"] = frameWidth;
            dict["frameHeight"] = frameHeight;
            dict["framesPerSecond"] = framesPerSecond;
            dict["framesDecoded"] = framesDecoded;
            dict["keyFramesDecoded"] = keyFramesDecoded;
            dict["framesDropped"] = framesDropped;
            dict["totalDecodeTime"] = totalDecodeTime;
            dict["totalProcessingDelay"] = totalProcessingDelay;
            dict["totalAssemblyTime"] = totalAssemblyTime;
            dict["framesAssembledFromMultiplePackets"] = framesAssembledFromMultiplePackets;
            dict["totalInterFrameDelay"] = totalInterFrameDelay;
            dict["totalSquaredInterFrameDelay"] = totalSquaredInterFrameDelay;
            dict["pauseCount"] = pauseCount;
            dict["totalPausesDuration"] = totalPausesDuration;
            dict["freezeCount"] = freezeCount;
            dict["totalFreezesDuration"] = totalFreezesDuration;
            dict["contentType"] = contentType;
            dict["estimatedPlayoutTimestamp"] = estimatedPlayoutTimestamp;
            dict["decoderImplementation"] = decoderImplementation;
            dict["firCount"] = firCount;
            dict["pliCount"] = pliCount;
            dict["nackCount"] = nackCount;
            dict["qpSum"] = qpSum;
            dict["totalCorruptionProbability"] = totalCorruptionProbability;
            dict["totalSquaredCorruptionProbability"] = totalSquaredCorruptionProbability;
            dict["corruptionMeasurements"] = corruptionMeasurements;
            dict["googTimingFrameInfo"] = googTimingFrameInfo;
            dict["powerEfficientDecoder"] = powerEfficientDecoder;
            dict["jitterBufferFlushes"] = jitterBufferFlushes;
            dict["delayedPacketOutageSamples"] = delayedPacketOutageSamples;
            dict["relativePacketArrivalDelay"] = relativePacketArrivalDelay;
            dict["interruptionCount"] = interruptionCount;
            dict["totalInterruptionDuration"] = totalInterruptionDuration;
            dict["minPlayoutDelay"] = minPlayoutDelay;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCOutboundRTPStreamStats : RTCSentRtpStreamStats
    {
        public string mediaSourceId { get; private set; }
        public string remoteId { get; private set; }
        public string mid { get; private set; }
        public string rid { get; private set; }
        public uint encodingIndex { get; private set; }
        public ulong retransmittedPacketsSent { get; private set; }
        public ulong headerBytesSent { get; private set; }
        public ulong retransmittedBytesSent { get; private set; }
        public double targetBitrate { get; private set; }
        public uint framesEncoded { get; private set; }
        public uint keyFramesEncoded { get; private set; }
        public double totalEncodeTime { get; private set; }
        public ulong totalEncodedBytesTarget { get; private set; }
        public uint frameWidth { get; private set; }
        public uint frameHeight { get; private set; }
        public double framesPerSecond { get; private set; }
        public uint framesSent { get; private set; }
        public uint hugeFramesSent { get; private set; }
        public double totalPacketSendDelay { get; private set; }
        public string qualityLimitationReason { get; private set; }
        public Dictionary<string, double> qualityLimitationDurations { get; private set; }
        public uint qualityLimitationResolutionChanges { get; private set; }
        public string contentType { get; private set; }
        public string encoderImplementation { get; private set; }
        public uint firCount { get; private set; }
        public uint pliCount { get; private set; }
        public uint nackCount { get; private set; }
        public ulong qpSum { get; private set; }
        public bool active { get; private set; }
        public bool powerEfficientEncoder { get; private set; }
        public string scalabilityMode { get; private set; }
        public uint rtxSsrc { get; private set; }

        internal RTCOutboundRTPStreamStats(in RTCOutboundRtpStreamStatsInternal statsInternal)
            : base(RTCStatsType.OutboundRtp, statsInternal.baseStats)
        {
            mediaSourceId = statsInternal.media_source_id;
            remoteId = statsInternal.remote_id;
            mid = statsInternal.mid;
            rid = statsInternal.rid;
            encodingIndex = statsInternal.encoding_index;
            retransmittedPacketsSent = statsInternal.retransmitted_packets_sent;
            headerBytesSent = statsInternal.header_bytes_sent;
            retransmittedBytesSent = statsInternal.retransmitted_bytes_sent;
            targetBitrate = statsInternal.target_bitrate;
            framesEncoded = statsInternal.frames_encoded;
            keyFramesEncoded = statsInternal.key_frames_encoded;
            totalEncodeTime = statsInternal.total_encode_time;
            totalEncodedBytesTarget = statsInternal.total_encoded_bytes_target;
            frameWidth = statsInternal.frame_width;
            frameHeight = statsInternal.frame_height;
            framesPerSecond = statsInternal.frames_per_second;
            framesSent = statsInternal.frames_sent;
            hugeFramesSent = statsInternal.huge_frames_sent;
            totalPacketSendDelay = statsInternal.total_packet_send_delay;
            qualityLimitationReason = statsInternal.quality_limitation_reason;

            if (!string.IsNullOrWhiteSpace(statsInternal.quality_limitation_durations))
            {
                qualityLimitationDurations = statsInternal.quality_limitation_durations
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => part.Split('='))
                    .Where(kv => kv.Length == 2)
                    .ToDictionary(
                        kv => kv[0].Trim(),
                        kv => double.Parse(kv[1].Trim())
                    );
            }
            qualityLimitationResolutionChanges = statsInternal.quality_limitation_resolution_changes;
            contentType = statsInternal.content_type;
            encoderImplementation = statsInternal.encoder_implementation;
            firCount = statsInternal.fir_count;
            pliCount = statsInternal.pli_count;
            nackCount = statsInternal.nack_count;
            qpSum = statsInternal.qp_sum;
            active = statsInternal.active;
            powerEfficientEncoder = statsInternal.power_efficient_encoder;
            scalabilityMode = statsInternal.scalability_mode;
            rtxSsrc = statsInternal.rtx_ssrc;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["mediaSourceId"] = mediaSourceId;
            dict["remoteId"] = remoteId;
            dict["mid"] = mid;
            dict["rid"] = rid;
            dict["encodingIndex"] = encodingIndex;
            dict["retransmittedPacketsSent"] = retransmittedPacketsSent;
            dict["headerBytesSent"] = headerBytesSent;
            dict["retransmittedBytesSent"] = retransmittedBytesSent;
            dict["targetBitrate"] = targetBitrate;
            dict["framesEncoded"] = framesEncoded;
            dict["keyFramesEncoded"] = keyFramesEncoded;
            dict["totalEncodeTime"] = totalEncodeTime;
            dict["totalEncodedBytesTarget"] = totalEncodedBytesTarget;
            dict["frameWidth"] = frameWidth;
            dict["frameHeight"] = frameHeight;
            dict["framesPerSecond"] = framesPerSecond;
            dict["framesSent"] = framesSent;
            dict["hugeFramesSent"] = hugeFramesSent;
            dict["totalPacketSendDelay"] = totalPacketSendDelay;
            dict["qualityLimitationReason"] = qualityLimitationReason;
            dict["qualityLimitationDurations"] = qualityLimitationDurations;
            dict["qualityLimitationResolutionChanges"] = qualityLimitationResolutionChanges;
            dict["contentType"] = contentType;
            dict["encoderImplementation"] = encoderImplementation;
            dict["firCount"] = firCount;
            dict["pliCount"] = pliCount;
            dict["nackCount"] = nackCount;
            dict["qpSum"] = qpSum;
            dict["active"] = active;
            dict["powerEfficientEncoder"] = powerEfficientEncoder;
            dict["scalabilityMode"] = scalabilityMode;
            dict["rtxSsrc"] = rtxSsrc;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCRemoteInboundRtpStreamStats : RTCReceivedRtpStreamStats
    {
        public string localId { get; private set; }
        public double roundTripTime { get; private set; }
        public double fractionLost { get; private set; }
        public double totalRoundTripTime { get; private set; }
        public int roundTripTimeMeasurements { get; private set; }

        internal RTCRemoteInboundRtpStreamStats(in RTCRemoteInboundRtpStreamStatsInternal statsInternal)
            : base(RTCStatsType.RemoteInboundRtp, statsInternal.baseStats)
        {
            localId = statsInternal.local_id;
            roundTripTime = statsInternal.round_trip_time;
            fractionLost = statsInternal.fraction_lost;
            totalRoundTripTime = statsInternal.total_round_trip_time;
            roundTripTimeMeasurements = statsInternal.round_trip_time_measurements;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["localId"] = localId;
            dict["roundTripTime"] = roundTripTime;
            dict["fractionLost"] = fractionLost;
            dict["totalRoundTripTime"] = totalRoundTripTime;
            dict["roundTripTimeMeasurements"] = roundTripTimeMeasurements;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCRemoteOutboundRtpStreamStats : RTCSentRtpStreamStats
    {
        public string localId { get; private set; }
        public double remoteTimestamp { get; private set; }
        public ulong reportsSent { get; private set; }
        public double roundTripTime { get; private set; }
        public ulong roundTripTimeMeasurements { get; private set; }
        public double totalRoundTripTime { get; private set; }

        internal RTCRemoteOutboundRtpStreamStats(in RTCRemoteOutboundRtpStreamStatsInternal statsInternal)
            : base(RTCStatsType.RemoteOutboundRtp, statsInternal.baseStats)
        {
            localId = statsInternal.local_id;
            remoteTimestamp = statsInternal.remote_timestamp;
            reportsSent = statsInternal.reports_sent;
            roundTripTime = statsInternal.round_trip_time;
            roundTripTimeMeasurements = statsInternal.round_trip_time_measurements;
            totalRoundTripTime = statsInternal.total_round_trip_time;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["localId"] = localId;
            dict["remoteTimestamp"] = remoteTimestamp;
            dict["reportsSent"] = reportsSent;
            dict["roundTripTime"] = roundTripTime;
            dict["roundTripTimeMeasurements"] = roundTripTimeMeasurements;
            dict["totalRoundTripTime"] = totalRoundTripTime;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCMediaSourceStats : RTCStats
    {
        public string trackIdentifier { get; private set; }
        public string kind { get; private set; }

        internal RTCMediaSourceStats(in RTCMediaSourceStatsInternal statsInternal)
            : base(RTCStatsType.MediaSource, statsInternal.rtc_stats)
        {
            trackIdentifier = statsInternal.track_identifier;
            kind = statsInternal.kind;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["trackIdentifier"] = trackIdentifier;
            dict["kind"] = kind;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCAudioSourceStats : RTCMediaSourceStats
    {
        public double audioLevel { get; private set; }
        public double totalAudioEnergy { get; private set; }
        public double totalSamplesDuration { get; private set; }
        public double echoReturnLoss { get; private set; }
        public double echoReturnLossEnhancement { get; private set; }

        internal RTCAudioSourceStats(in RTCMediaSourceStatsInternal statsInternal)
            : base(statsInternal)
        {
            audioLevel = statsInternal.audio_level;
            totalAudioEnergy = statsInternal.total_audio_energy;
            totalSamplesDuration = statsInternal.total_samples_duration;
            echoReturnLoss = statsInternal.echo_return_loss;
            echoReturnLossEnhancement = statsInternal.echo_return_loss_enhancement;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["audioLevel"] = audioLevel;
            dict["totalAudioEnergy"] = totalAudioEnergy;
            dict["totalSamplesDuration"] = totalSamplesDuration;
            dict["echoReturnLoss"] = echoReturnLoss;
            dict["echoReturnLossEnhancement"] = echoReturnLossEnhancement;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCVideoSourceStats : RTCMediaSourceStats
    {
        public uint width { get; private set; }
        public uint height { get; private set; }
        public uint frames { get; private set; }
        public double framesPerSecond { get; private set; }

        internal RTCVideoSourceStats(in RTCMediaSourceStatsInternal statsInternal)
            : base(statsInternal)
        {
            width = statsInternal.width;
            height = statsInternal.height;
            frames = statsInternal.frames;
            framesPerSecond = statsInternal.frames_per_second;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["width"] = width;
            dict["height"] = height;
            dict["frames"] = frames;
            dict["framesPerSecond"] = framesPerSecond;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCTransportStats : RTCStats
    {
        public ulong bytesSent { get; private set; }
        public ulong packetsSent { get; private set; }
        public ulong bytesReceived { get; private set; }
        public ulong packetsReceived { get; private set; }
        public string rtcpTransportStatsId { get; private set; }
        public string dtlsState { get; private set; }
        public string selectedCandidatePairId { get; private set; }
        public string localCertificateId { get; private set; }
        public string remoteCertificateId { get; private set; }
        public string tlsVersion { get; private set; }
        public string dtlsCipher { get; private set; }
        public string dtlsRole { get; private set; }
        public string srtpCipher { get; private set; }
        public uint selectedCandidatePairChanges { get; private set; }
        public string iceRole { get; private set; }
        public string iceLocalUsernameFragment { get; private set; }
        public string iceState { get; private set; }

        internal RTCTransportStats(in RTCTransportStatsInternal statsInternal)
            : base(RTCStatsType.Transport, statsInternal.rtc_stats)
        {
            bytesSent = statsInternal.bytes_sent;
            packetsSent = statsInternal.packets_sent;
            bytesReceived = statsInternal.bytes_received;
            packetsReceived = statsInternal.packets_received;
            rtcpTransportStatsId = statsInternal.rtcp_transport_stats_id;
            dtlsState = statsInternal.dtls_state;
            selectedCandidatePairId = statsInternal.selected_candidate_pair_id;
            localCertificateId = statsInternal.local_certificate_id;
            remoteCertificateId = statsInternal.remote_certificate_id;
            tlsVersion = statsInternal.tls_version;
            dtlsCipher = statsInternal.dtls_cipher;
            dtlsRole = statsInternal.dtls_role;
            srtpCipher = statsInternal.srtp_cipher;
            selectedCandidatePairChanges = statsInternal.selected_candidate_pair_changes;
            iceRole = statsInternal.ice_role;
            iceLocalUsernameFragment = statsInternal.ice_local_username_fragment;
            iceState = statsInternal.ice_state;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["bytesSent"] = bytesSent;
            dict["packetsSent"] = packetsSent;
            dict["bytesReceived"] = bytesReceived;
            dict["packetsReceived"] = packetsReceived;
            dict["rtcpTransportStatsId"] = rtcpTransportStatsId;
            dict["dtlsState"] = dtlsState;
            dict["selectedCandidatePairId"] = selectedCandidatePairId;
            dict["localCertificateId"] = localCertificateId;
            dict["remoteCertificateId"] = remoteCertificateId;
            dict["tlsVersion"] = tlsVersion;
            dict["dtlsCipher"] = dtlsCipher;
            dict["dtlsRole"] = dtlsRole;
            dict["srtpCipher"] = srtpCipher;
            dict["selectedCandidatePairChanges"] = selectedCandidatePairChanges;
            dict["iceRole"] = iceRole;
            dict["iceLocalUsernameFragment"] = iceLocalUsernameFragment;
            dict["iceState"] = iceState;
            return dict;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCAudioPlayoutStats : RTCStats
    {
        public string kind { get; private set; }
        public double synthesizedSamplesDuration { get; private set; }
        public ulong synthesizedSamplesEvents { get; private set; }
        public double totalSamplesDuration { get; private set; }
        public double totalPlayoutDelay { get; private set; }
        public ulong totalSamplesCount { get; private set; }

        internal RTCAudioPlayoutStats(in RTCAudioPlayoutStatsInternal statsInternal)
            : base(RTCStatsType.MediaPlayOut, statsInternal.rtc_stats)
        {
            kind = statsInternal.kind;
            synthesizedSamplesDuration = statsInternal.synthesized_samples_duration;
            synthesizedSamplesEvents = statsInternal.synthesized_samples_events;
            totalSamplesDuration = statsInternal.total_samples_duration;
            totalPlayoutDelay = statsInternal.total_playout_delay;
            totalSamplesCount = statsInternal.total_samples_count;
        }
        protected override Dictionary<string, object> BuildAttributeMap()
        {
            var dict = base.BuildAttributeMap();
            dict["kind"] = kind;
            dict["synthesizedSamplesDuration"] = synthesizedSamplesDuration;
            dict["synthesizedSamplesEvents"] = synthesizedSamplesEvents;
            dict["totalSamplesDuration"] = totalSamplesDuration;
            dict["totalPlayoutDelay"] = totalPlayoutDelay;
            dict["totalSamplesCount"] = totalSamplesCount;
            return dict;
        }
    }

    internal class StatsFactory
    {
        static Dictionary<RTCStatsType, Func<IntPtr, RTCStats>> m_map;

        static StatsFactory()
        {
            m_map = new Dictionary<RTCStatsType, Func<IntPtr, RTCStats>>()
            {
                {
                    RTCStatsType.Certificate,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCCertificateStatsInternal>(ptr);
                        return new RTCCertificateStats(in s);
                    }
                },
                {
                    RTCStatsType.Codec,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCCodecStatsInternal>(ptr);
                        return new RTCCodecStats(in s);
                    }
                },
                {
                    RTCStatsType.DataChannel,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCDataChannelStatsInternal>(ptr);
                        return new RTCDataChannelStats(in s);
                    }
                },
                {
                    RTCStatsType.CandidatePair,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCIceCandidatePairStatsInternal>(ptr);
                        return new RTCIceCandidatePairStats(in s);
                    }
                },
                {
                    RTCStatsType.LocalCandidate,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCIceCandidateStatsInternal>(ptr);
                        return new RTCIceCandidateStats(in s);
                    }
                },
                {
                    RTCStatsType.RemoteCandidate,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCIceCandidateStatsInternal>(ptr);
                        return new RTCIceCandidateStats(in s);
                    }
                },
                {
                    RTCStatsType.PeerConnection,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCPeerConnectionStatsInternal>(ptr);
                        return new RTCPeerConnectionStats(in s);
                    }
                },
                {
                    RTCStatsType.InboundRtp,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCInboundRtpStreamStatsInternal>(ptr);
                        return new RTCInboundRTPStreamStats(in s);
                    }
                },
                {
                    RTCStatsType.OutboundRtp,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCOutboundRtpStreamStatsInternal>(ptr);
                        return new RTCOutboundRTPStreamStats(in s);
                    }
                },
                {
                    RTCStatsType.RemoteInboundRtp,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCRemoteInboundRtpStreamStatsInternal>(ptr);
                        return new RTCRemoteInboundRtpStreamStats(in s);
                    }
                },
                {
                    RTCStatsType.RemoteOutboundRtp,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCRemoteOutboundRtpStreamStatsInternal>(ptr);
                        return new RTCRemoteOutboundRtpStreamStats(in s);
                    }
                },
                {
                    RTCStatsType.MediaSource,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCMediaSourceStatsInternal>(ptr);
                        return s.kind == "audio" ? new RTCAudioSourceStats(in s) : new RTCVideoSourceStats(in s);
                    }
                },
                {
                    RTCStatsType.Transport,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCTransportStatsInternal>(ptr);
                        return new RTCTransportStats(in s);
                    }
                },
                {
                    RTCStatsType.MediaPlayOut,
                    ptr =>
                    {
                        var s = Marshal.PtrToStructure<RTCAudioPlayoutStatsInternal>(ptr);
                        return new RTCAudioPlayoutStats(in s);
                    }
                },
            };
        }

        public static RTCStats Create(RTCStatsType type, IntPtr ptr)
        {
            return m_map.TryGetValue(type, out var constructor) ? constructor(ptr) : null;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RTCStatsReport : IDisposable
    {
        private IntPtr self;
        private readonly Dictionary<string, RTCStats> m_dictStats;

        private bool disposed;

        internal RTCStatsReport(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                throw new ArgumentException("Invalid stats report ptr");
            }
            self = ptr;
            m_dictStats = new Dictionary<string, RTCStats>();

            IntPtr ptrStatsTypeArray = IntPtr.Zero;
            IntPtr ptrStatsArray = WebRTC.Context.GetStatsList(self, out ulong length, ref ptrStatsTypeArray);
            if (length == 0)
            {
                // allow this?
                return;
            }
            if (ptrStatsArray == IntPtr.Zero
                || ptrStatsTypeArray == IntPtr.Zero)
            {
                throw new ArgumentException("Invalid stats list");
            }

            IntPtr[] array = ptrStatsArray.AsArray<IntPtr>((int)length);
            uint[] types = ptrStatsTypeArray.AsArray<uint>((int)length);

            for (int i = 0; i < (int)length; i++)
            {
                RTCStatsType type = (RTCStatsType)types[i];
                RTCStats stats = StatsFactory.Create(type, array[i]);
                NativeMethods.ReleaseStats(array[i], (int)type);
                if (stats == null)
                {
                    Debug.LogError($"Failed to create RTCStats: {type}");
                    continue;
                }
                m_dictStats[stats.Id] = stats;

                // Debug.Log(
                //     $"RTCStats: {type} :\n" +
                //     string.Join("\n", stats.Dict.Select(kvp =>
                //         $"    {kvp.Key}: " + (kvp.Value switch
                //         {
                //             null => "null",
                //             Dictionary<string, double> nested =>
                //                 "{" + string.Join(", ", nested.Select(n => $"{n.Key}={n.Value}")) + "}",
                //             _ => kvp.Value.ToString()
                //         })
                //     ))
                // );

                // Debug.Log($"json : {stats.ToJson()}");
            }

            WebRTC.Table.Add(self, this);
        }

        /// <summary>
        ///
        /// </summary>
        ~RTCStatsReport()
        {
            this.Dispose();
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            if (self != IntPtr.Zero && !WebRTC.Context.IsNull)
            {
                WebRTC.Context.DeleteStatsReport(self);
                WebRTC.Table.Remove(self);
                self = IntPtr.Zero;
            }

            this.disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RTCStats Get(string id)
        {
            return m_dictStats[id];
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stats"></param>
        /// <returns></returns>
        public bool TryGetValue(string id, out RTCStats stats)
        {
            return m_dictStats.TryGetValue(id, out stats);
        }

        /// <summary>
        ///
        /// </summary>
        public IDictionary<string, RTCStats> Stats
        {
            get { return m_dictStats; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCStatsInternal
    {

        [MarshalAs(UnmanagedType.LPStr)]
        public string id;

        public long timestamp;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCCertificateStatsInternal
    {
        public RTCStatsInternal rtc_stats;

        [MarshalAs(UnmanagedType.LPStr)]
        public string fingerprint;

        [MarshalAs(UnmanagedType.LPStr)]
        public string fingerprint_algorithm;

        [MarshalAs(UnmanagedType.LPStr)]
        public string base64_certificate;

        [MarshalAs(UnmanagedType.LPStr)]
        public string issuer_certificate_id;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCCodecStatsInternal
    {
        public RTCStatsInternal rtc_stats;

        [MarshalAs(UnmanagedType.LPStr)]
        public string transport_id;

        public uint payload_type;

        [MarshalAs(UnmanagedType.LPStr)]
        public string mime_type;

        public uint clock_rate;
        public uint channels;

        [MarshalAs(UnmanagedType.LPStr)]
        public string sdp_fmtp_line;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCDataChannelStatsInternal
    {
        public RTCStatsInternal rtc_stats;

        [MarshalAs(UnmanagedType.LPStr)]
        public string label;

        [MarshalAs(UnmanagedType.LPStr)]
        public string protocol;

        public int data_channel_identifier;

        [MarshalAs(UnmanagedType.LPStr)]
        public string state;

        public uint messages_sent;
        public ulong bytes_sent;
        public uint messages_received;
        public ulong bytes_received;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCIceCandidatePairStatsInternal
    {
        public RTCStatsInternal rtc_stats;

        [MarshalAs(UnmanagedType.LPStr)]
        public string transport_id;

        [MarshalAs(UnmanagedType.LPStr)]
        public string local_candidate_id;

        [MarshalAs(UnmanagedType.LPStr)]
        public string remote_candidate_id;

        [MarshalAs(UnmanagedType.LPStr)]
        public string state;

        public ulong priority;
        public bool nominated;
        public bool writable;
        public ulong packets_sent;
        public ulong packets_received;
        public ulong bytes_sent;
        public ulong bytes_received;
        public double total_round_trip_time;
        public double current_round_trip_time;
        public double available_outgoing_bitrate;
        public double available_incoming_bitrate;
        public ulong requests_received;
        public ulong requests_sent;
        public ulong responses_received;
        public ulong responses_sent;
        public ulong consent_requests_sent;
        public ulong packets_discarded_on_send;
        public ulong bytes_discarded_on_send;
        public double last_packet_received_timestamp;
        public double last_packet_sent_timestamp;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCIceCandidateStatsInternal
    {
        public RTCStatsInternal rtc_stats;

        [MarshalAs(UnmanagedType.LPStr)]
        public string transport_id;

        public bool is_remote;

        [MarshalAs(UnmanagedType.LPStr)]
        public string network_type;

        [MarshalAs(UnmanagedType.LPStr)]
        public string ip;

        [MarshalAs(UnmanagedType.LPStr)]
        public string address;

        public int port;

        [MarshalAs(UnmanagedType.LPStr)]
        public string protocol;

        [MarshalAs(UnmanagedType.LPStr)]
        public string relay_protocol;

        [MarshalAs(UnmanagedType.LPStr)]
        public string candidate_type;

        public int priority;

        [MarshalAs(UnmanagedType.LPStr)]
        public string url;

        [MarshalAs(UnmanagedType.LPStr)]
        public string foundation;

        [MarshalAs(UnmanagedType.LPStr)]
        public string related_address;

        public int related_port;

        [MarshalAs(UnmanagedType.LPStr)]
        public string username_fragment;

        [MarshalAs(UnmanagedType.LPStr)]
        public string tcp_type;

        public bool vpn;

        [MarshalAs(UnmanagedType.LPStr)]
        public string network_adapter_type;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCPeerConnectionStatsInternal
    {
        public RTCStatsInternal rtc_stats;
        public uint data_channels_opened;
        public uint data_channels_closed;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCRtpStreamStatsInternal
    {
        public RTCStatsInternal rtc_stats;
        public uint ssrc;

        [MarshalAs(UnmanagedType.LPStr)]
        public string kind;

        [MarshalAs(UnmanagedType.LPStr)]
        public string transport_id;

        [MarshalAs(UnmanagedType.LPStr)]
        public string codec_id;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCReceivedRtpStreamStatsInternal
    {
        public RTCRtpStreamStatsInternal baseStats;
        public double jitter;
        public int packets_lost;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCSentRtpStreamStatsInternal
    {
        public RTCRtpStreamStatsInternal baseStats;
        public ulong packets_sent;
        public ulong bytes_sent;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCInboundRtpStreamStatsInternal
    {
        public RTCReceivedRtpStreamStatsInternal baseStats;

        [MarshalAs(UnmanagedType.LPStr)]
        public string playout_id;

        [MarshalAs(UnmanagedType.LPStr)]
        public string track_identifier;

        [MarshalAs(UnmanagedType.LPStr)]
        public string mid;

        [MarshalAs(UnmanagedType.LPStr)]
        public string remote_id;

        public uint packets_received;
        public ulong packets_discarded;
        public ulong fec_packets_received;
        public ulong fec_bytes_received;
        public ulong fec_packets_discarded;
        public uint fec_ssrc;
        public ulong bytes_received;
        public ulong header_bytes_received;
        public ulong retransmitted_packets_received;
        public ulong retransmitted_bytes_received;
        public uint rtx_ssrc;
        public double last_packet_received_timestamp;
        public double jitter_buffer_delay;
        public double jitter_buffer_target_delay;
        public double jitter_buffer_minimum_delay;
        public ulong jitter_buffer_emitted_count;
        public ulong total_samples_received;
        public ulong concealed_samples;
        public ulong silent_concealed_samples;
        public ulong concealment_events;
        public ulong inserted_samples_for_deceleration;
        public ulong removed_samples_for_acceleration;
        public double audio_level;
        public double total_audio_energy;
        public double total_samples_duration;
        public uint frames_received;
        public uint frame_width;
        public uint frame_height;
        public double frames_per_second;
        public uint frames_decoded;
        public uint key_frames_decoded;
        public uint frames_dropped;
        public double total_decode_time;
        public double total_processing_delay;
        public double total_assembly_time;
        public uint frames_assembled_from_multiple_packets;
        public double total_inter_frame_delay;
        public double total_squared_inter_frame_delay;
        public uint pause_count;
        public double total_pauses_duration;
        public uint freeze_count;
        public double total_freezes_duration;

        [MarshalAs(UnmanagedType.LPStr)]
        public string content_type;

        public double estimated_playout_timestamp;

        [MarshalAs(UnmanagedType.LPStr)]
        public string decoder_implementation;

        public uint fir_count;
        public uint pli_count;
        public uint nack_count;
        public ulong qp_sum;
        public double total_corruption_probability;
        public double total_squared_corruption_probability;
        public ulong corruption_measurements;

        [MarshalAs(UnmanagedType.LPStr)]
        public string goog_timing_frame_info;

        public bool power_efficient_decoder;
        public ulong jitter_buffer_flushes;
        public ulong delayed_packet_outage_samples;
        public double relative_packet_arrival_delay;
        public uint interruption_count;
        public double total_interruption_duration;
        public double min_playout_delay;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCOutboundRtpStreamStatsInternal
    {
        public RTCSentRtpStreamStatsInternal baseStats;

        [MarshalAs(UnmanagedType.LPStr)]
        public string media_source_id;

        [MarshalAs(UnmanagedType.LPStr)]
        public string remote_id;

        [MarshalAs(UnmanagedType.LPStr)]
        public string mid;

        [MarshalAs(UnmanagedType.LPStr)]
        public string rid;

        public uint encoding_index;
        public ulong retransmitted_packets_sent;
        public ulong header_bytes_sent;
        public ulong retransmitted_bytes_sent;
        public double target_bitrate;
        public uint frames_encoded;
        public uint key_frames_encoded;
        public double total_encode_time;
        public ulong total_encoded_bytes_target;
        public uint frame_width;
        public uint frame_height;
        public double frames_per_second;
        public uint frames_sent;
        public uint huge_frames_sent;
        public double total_packet_send_delay;

        [MarshalAs(UnmanagedType.LPStr)]
        public string quality_limitation_reason;

        [MarshalAs(UnmanagedType.LPStr)]
        public string quality_limitation_durations;

        public uint quality_limitation_resolution_changes;

        [MarshalAs(UnmanagedType.LPStr)]
        public string content_type;

        [MarshalAs(UnmanagedType.LPStr)]
        public string encoder_implementation;

        public uint fir_count;
        public uint pli_count;
        public uint nack_count;
        public ulong qp_sum;
        public bool active;
        public bool power_efficient_encoder;

        [MarshalAs(UnmanagedType.LPStr)]
        public string scalability_mode;

        public uint rtx_ssrc;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCRemoteInboundRtpStreamStatsInternal
    {
        public RTCReceivedRtpStreamStatsInternal baseStats;

        [MarshalAs(UnmanagedType.LPStr)]
        public string local_id;

        public double round_trip_time;
        public double fraction_lost;
        public double total_round_trip_time;
        public int round_trip_time_measurements;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCRemoteOutboundRtpStreamStatsInternal
    {
        public RTCSentRtpStreamStatsInternal baseStats;

        [MarshalAs(UnmanagedType.LPStr)]
        public string local_id;

        public double remote_timestamp;
        public ulong reports_sent;
        public double round_trip_time;
        public ulong round_trip_time_measurements;
        public double total_round_trip_time;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCMediaSourceStatsInternal
    {
        public RTCStatsInternal rtc_stats;

        [MarshalAs(UnmanagedType.LPStr)]
        public string track_identifier;

        [MarshalAs(UnmanagedType.LPStr)]
        public string kind;

        public double audio_level;
        public double total_audio_energy;
        public double total_samples_duration;
        public double echo_return_loss;
        public double echo_return_loss_enhancement;
        public uint width;
        public uint height;
        public uint frames;
        public double frames_per_second;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCTransportStatsInternal
    {
        public RTCStatsInternal rtc_stats;
        public ulong bytes_sent;
        public ulong packets_sent;
        public ulong bytes_received;
        public ulong packets_received;

        [MarshalAs(UnmanagedType.LPStr)]
        public string rtcp_transport_stats_id;

        [MarshalAs(UnmanagedType.LPStr)]
        public string dtls_state;

        [MarshalAs(UnmanagedType.LPStr)]
        public string selected_candidate_pair_id;

        [MarshalAs(UnmanagedType.LPStr)]
        public string local_certificate_id;

        [MarshalAs(UnmanagedType.LPStr)]
        public string remote_certificate_id;

        [MarshalAs(UnmanagedType.LPStr)]
        public string tls_version;

        [MarshalAs(UnmanagedType.LPStr)]
        public string dtls_cipher;

        [MarshalAs(UnmanagedType.LPStr)]
        public string dtls_role;

        [MarshalAs(UnmanagedType.LPStr)]
        public string srtp_cipher;

        public uint selected_candidate_pair_changes;

        [MarshalAs(UnmanagedType.LPStr)]
        public string ice_role;

        [MarshalAs(UnmanagedType.LPStr)]
        public string ice_local_username_fragment;

        [MarshalAs(UnmanagedType.LPStr)]
        public string ice_state;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCAudioPlayoutStatsInternal
    {
        public RTCStatsInternal rtc_stats;

        [MarshalAs(UnmanagedType.LPStr)]
        public string kind;

        public double synthesized_samples_duration;
        public ulong synthesized_samples_events;
        public double total_samples_duration;
        public double total_playout_delay;
        public ulong total_samples_count;
    }
}
