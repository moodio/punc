CREATE TABLE do_not_disturb(
    email text UNIQUE,
    date_created_utc timestamp without time zone default timezone('utc', now())
);