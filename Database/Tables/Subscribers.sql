CREATE TABLE subscribers(
    email citext PRIMARY KEY,
    date_created_utc timestamp without time zone default timezone('utc', now())
);