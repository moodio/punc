CREATE TABLE confirmation_secrets(
    timers_id uuid NOT NULL,
    code text NOT NULL,
    created date NOT NULL DEFAULT timezone('utc', now())
);