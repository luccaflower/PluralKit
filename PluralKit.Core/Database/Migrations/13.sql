CREATE TABLE IF NOT EXISTS reminders
(
    mid bigint NOT NULL,
    channel bigint NOT NULL,
    guild bigint NOT NULL,
    member integer,
    seen boolean NOT NULL,
    system integer NOT NULL,
    timestamp timestamp NOT NULL default (current_timestamp at time zone 'utc'),
    CONSTRAINT reminders_pkey PRIMARY KEY (mid),
    CONSTRAINT reminders_receiver_fkey FOREIGN KEY (member)
        REFERENCES members (id)
        ON DELETE CASCADE
)