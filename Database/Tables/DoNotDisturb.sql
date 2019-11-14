CREATE TABLE do_not_disturb(
    email text UNIQUE,
    date_created_utc timestamp without time zone default timezone('utc', now())
);

SELECT * FROM subscribers;
SELECT * FROM do_not_disturb;

SELECT * FROM timers;

SELECT * FROM subscribers;

SELECT * FROM timers;