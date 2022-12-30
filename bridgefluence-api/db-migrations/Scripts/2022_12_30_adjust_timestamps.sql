--liquibase formatted sql
--changeset andrey:2022_12_30_adjust_timestamps.sql
ALTER TABLE public.post
ALTER COLUMN submitted_at TYPE timestamp without time zone;

ALTER TABLE public.post
ALTER COLUMN postponed_at TYPE timestamp without time zone;

ALTER TABLE public.post
ALTER COLUMN rejected_at TYPE timestamp without time zone;

ALTER TABLE public.post
ALTER COLUMN paid_at TYPE timestamp without time zone;