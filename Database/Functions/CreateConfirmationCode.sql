CREATE OR REPLACE FUNCTION create_referee_code(
    "TimerId" uuid,
    "Code" text
) RETURNS boolean AS $$
BEGIN
    INSERT INTO referee_codes(timers_id, code)
    VALUES($1, $2);
    RETURN true;
END;
$$  LANGUAGE PLPGSQL
    SECURITY definer
    SET search_path = public;
