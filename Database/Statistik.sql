# gemeldeten L5-Raids
SELECT count(*) as count
FROM POGO_RAIDS
WHERE START < '2018-06-01' AND START > '2018-05-01'
AND LEVEL = 5
ORDER BY START asc
;


# beliebtesten Raids (absagen raus gefiltert)
SELECT start, name, count, y.POKE_ID
FROM
(select p.RAID_ID as id , count(*) as count
From POGO_RAIDS r INNER JOIN ACTIVE_POLLS p ON p.RAID_ID = r.ID INNER JOIN USER_VOTES v ON p.ID = v.POLL_ID
where r.START < '2018-11-01' AND r.START > '2018-10-01' AND p.CHAT_ID = -1001383122714 AND v.ATTENDEE <> 0
Group by p.RAID_ID
order by count(*) desc) x JOIN POGO_RAIDS y ON x.id = y.ID INNER JOIN POGO_GYMS g on y.GYM_ID = g.ID
;

# fleißigsten Melder
select u.INGAME_NAME as name, count(*) as count
FROM POGO_RAIDS r INNER JOIN POGO_USER u ON r.OWNER_ID = u.USER_ID
WHERE r.START < '2018-11-01' AND r.START > '2018-10-01'
GROUP BY u.INGAME_NAME
order by count(*) desc
;

# meisten gemeldet Gym
SELECT g2.NAME, count
FROM
(select g.ID as ID, count(*) as count
FROM POGO_RAIDS r INNER JOIN POGO_GYMS g ON r.GYM_ID = g.ID
WHERE r.START < '2018-11-01' AND r.START > '2018-10-01'
group by g.ID
order by count(*) desc) y INNER JOIN POGO_GYMS g2 ON y.ID = g2.ID
;

# Botnutzer
SELECT u.INGAME_NAME as name, count
FROM
(select v.USER_ID, count(*) as count
From POGO_RAIDS r INNER JOIN ACTIVE_POLLS p ON p.RAID_ID = r.ID INNER JOIN USER_VOTES v ON p.ID = v.POLL_ID
where r.START < '2018-11-01' AND r.START > '2018-10-01' AND v.ATTENDEE <> 0 AND p.CHAT_ID = -1001383122714
group by v.USER_ID) y INNER JOIN POGO_USER u ON y.USER_ID = u.USER_ID
order by count desc, name asc
;
# Letzter Zugriff aller Nutzer
SELECT USER.USER_ID, FIRST_NAME AS TELEGRAM_NAME, INGAME_NAME, LEVEL, TEAM, ACT.*
FROM 
(
SELECT u.*
FROM POGO_USER u INNER JOIN MEMBERSHIPS m ON u.USER_ID = m.USER_ID
WHERE m.GROUP_ID =  -1001383122714
) AS USER LEFT OUTER JOIN 
(
select v.USER_ID, MAX(r.START) as LAST_VOTE, count(*) as ACTIONS
From POGO_RAIDS r INNER JOIN ACTIVE_POLLS p ON p.RAID_ID = r.ID INNER JOIN USER_VOTES v ON p.ID = v.POLL_ID
where /* r.START < '2018-10-01' AND r.START > '2018-09-01' AND */ p.CHAT_ID = -1001383122714
group by v.USER_ID
) AS ACT ON USER.USER_ID = ACT.USER_ID
ORDER BY LAST_VOTE, USER.USER_ID

