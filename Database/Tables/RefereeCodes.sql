CREATE TABLE referee_codes(
    timers_id uuid NOT NULL,
    code text NOT NULL,
    created timestamp NOT NULL DEFAULT timezone('utc', now())
);