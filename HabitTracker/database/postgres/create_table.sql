CREATE TABLE "habit"
(
    id          UUID PRIMARY KEY,
    name        VARCHAR(100) NOT NULL,
    user_id     UUID NOT NULL,
    created_at  TIMESTAMPTZ NOT NULL,
	deleted_at 	TIMESTAMPTZ
);

CREATE TABLE "logs"
(
    id         UUID PRIMARY KEY NOT NULL,
    habit_id   UUID NOT NULL,
    user_id    UUID NOT NULL,
    logDate    TIMESTAMPTZ,
    state      JSONB,
	deleted_at TIMESTAMPTZ,
    FOREIGN KEY (habit_id) REFERENCES "habit" (id)
);


CREATE TABLE "days_off"
(
    habit_id   UUID NOT NULL,
    off_days   VARCHAR(3),
	deleted_at TIMESTAMPTZ,
    FOREIGN KEY (habit_id) REFERENCES "habit" (id)
);

CREATE TABLE "badge"
(
    id              UUID NOT NULL PRIMARY KEY,
    name            VARCHAR(100) NOT NULL,
    description     VARCHAR(100) NOT NULL,
    user_id         UUID NOT NULL,

    created_at      TIMESTAMPTZ NOT NULL
);


CREATE TABLE "habit_snapshot" 
( 
    id                      UUID PRIMARY KEY, 
    habit_id                UUID NOT NULL, 
    log_count               INT NOT NULL, 
    user_id                 UUID NOT NULL,
    last_log_id             UUID NOT NULL, 
    last_log_created_at     TIMESTAMPTZ NOT NULL, 
    created_at              TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP, 
    
    FOREIGN KEY (habit_id) REFERENCES "habit" (id),
    FOREIGN KEY (last_log_id) REFERENCES "logs" (id)
); 