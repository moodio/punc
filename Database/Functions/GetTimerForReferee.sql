CREATE OR REPLACE FUNCTION get_referee_timer_id(
    "Code" text
) RETURNS uuid AS $$
DECLARE
    v_timers_id uuid;
BEGIN
    SELECT timers_id FROM referee_codes
    WHERE code = $1
    INTO v_timers_id;
    return v_timers_id;
END;

$$  LANGUAGE plpgsql
    SECURITY DEFINER
    SET SEARCH_PATH = public;
