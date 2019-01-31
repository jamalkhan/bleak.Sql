SELECT
    zid
,   shopper_id
,   session_start_date
,   session_end_date
,   session_length_minutes
,   session_length_seconds
,   is_purchase
FROM
(
    SELECT DISTINCT
        zid
    ,   shopper_id
    ,   CAST(NULL AS Datetime) AS session_start_date
    ,   session_end_date
    ,   CAST(NULL AS BIGINT) AS session_length_minutes
    ,   CAST(NULL AS BIGINT) AS session_length_seconds
    ,   1 AS is_purchase
    FROM
    (
        SELECT
            purchase_events.sid
        ,   purchase_events.zid
        ,   DATEADD(ms, purchase_events.createdat - DATEDIFF(ms, '1970-01-01', GETDATE()), GETDATE()) AS session_end_date
        ,   si3.shopper_id
        FROM
        (
            SELECT
                sid
            ,   zid
            ,   createdat
            FROM data_onboarding.web_purchase_confirm
            WHERE zid IS NOT NULL
        ) purchase_events
        JOIN
        (
            SELECT
                shopper_id
            ,   id_value AS zid
            FROM shopper360.shopper_identifier
            WHERE id_type = 'zid'
        ) si3
        ON si3.zid = purchase_events.zid
    ) synthesized_purchase_events
) ins_table