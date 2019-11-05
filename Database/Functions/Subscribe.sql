CREATE OR REPLACE FUNCTION subscribe(
    "Email" text
) RETURNS bool as $$
BEGIN
    DELETE FROM do_not_disturb
    WHERE email = $1::citext;
    IF NOT EXISTS (SELECT 1 FROM subscribers WHERE email = $1::citext) THEN
        INSERT INTO subscribers(email)
        VALUES($1::citext);
    END IF;
    RETURN TRUE;
END;
$$  LANGUAGE plpgsql
    SECURITY DEFINER
    SET SEARCH_PATH = public;
