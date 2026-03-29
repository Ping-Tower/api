CREATE TABLE IF NOT EXISTS pingtower_analytics.server_pings
(
    id UUID,
    server_id String,
    protocol String,
    timestamp DateTime64(3, 'UTC'),
    is_success Bool,
    latency_ms Nullable(Float64),
    error_message Nullable(String),
    status_code Nullable(Int32),
    cert_expires_at Nullable(DateTime64(3, 'UTC')),
    tls_version Nullable(String),
    dns_lookup_ms Nullable(Float64),
    sent_bytes Nullable(Int64),
    received_bytes Nullable(Int64),
    packet_loss_percent Nullable(Float64),
    rtt_min_ms Nullable(Float64),
    rtt_max_ms Nullable(Float64),
    ttl Nullable(Int32)
)
ENGINE = MergeTree
PARTITION BY toYYYYMMDD(timestamp)
ORDER BY (server_id, timestamp)
TTL timestamp + INTERVAL 30 DAY;
