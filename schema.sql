--
-- PostgreSQL database dump
--

\restrict asudw8LNi6NMjXxNW8mpWzE61qd9h6Po9UDBy3IkwFLODZk8pqKuVaeX1nZSM5y

-- Dumped from database version 17.6
-- Dumped by pg_dump version 18.3

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Accounts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Accounts" (
    "Id" uuid NOT NULL,
    "FullName" character varying(200) NOT NULL,
    "UserName" character varying(100) NOT NULL,
    "EmailAddress" character varying(255) NOT NULL,
    "HashPassword" text NOT NULL,
    "Gender" integer NOT NULL,
    "PhoneNumber" character varying(20),
    "Birthday" date,
    "Role" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "LastLogin" timestamp with time zone,
    "RefreshToken" text,
    "RefreshTokenExpiryTime" timestamp with time zone,
    "IsActive" boolean NOT NULL,
    "EmailConfirmed" boolean NOT NULL,
    "EmailVerificationToken" text,
    "EmailVerificationTokenExpiry" timestamp with time zone,
    "PasswordResetToken" text,
    "PasswordResetTokenExpiry" timestamp with time zone
);


ALTER TABLE public."Accounts" OWNER TO postgres;

--
-- Name: Bosses; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Bosses" (
    "Id" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Level" integer NOT NULL,
    "Health" integer NOT NULL,
    "Attack" integer NOT NULL,
    "Defense" integer NOT NULL,
    "SpecialSkillDescription" text,
    "IsFinalBoss" boolean NOT NULL
);


ALTER TABLE public."Bosses" OWNER TO postgres;

--
-- Name: EquipmentStats; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."EquipmentStats" (
    "Id" uuid NOT NULL,
    "ItemId" uuid NOT NULL,
    "HealthBonus" integer NOT NULL,
    "ManaBonus" integer NOT NULL,
    "StrengthBonus" integer NOT NULL,
    "DefenseBonus" integer NOT NULL,
    "AgilityBonus" integer NOT NULL,
    "IntelligenceBonus" integer NOT NULL,
    "EnduranceBonus" integer NOT NULL,
    "LuckBonus" integer NOT NULL,
    "AttackBonus" integer NOT NULL,
    "CriticalRateBonus" integer NOT NULL,
    "CriticalDamageBonus" integer NOT NULL,
    "ArmorPenetrationBonus" integer NOT NULL
);


ALTER TABLE public."EquipmentStats" OWNER TO postgres;

--
-- Name: Friends; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Friends" (
    "Id" uuid NOT NULL,
    "RequesterId" uuid NOT NULL,
    "AddresseeId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "RespondedAt" timestamp with time zone
);


ALTER TABLE public."Friends" OWNER TO postgres;

--
-- Name: GachaBannerItems; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."GachaBannerItems" (
    "Id" uuid NOT NULL,
    "GachaBannerId" uuid NOT NULL,
    "ItemId" uuid NOT NULL,
    "DropRate" numeric NOT NULL,
    "IsFeatured" boolean NOT NULL,
    "ItemId1" uuid
);


ALTER TABLE public."GachaBannerItems" OWNER TO postgres;

--
-- Name: GachaBanners; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."GachaBanners" (
    "Id" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Type" integer NOT NULL,
    "PullCost" integer NOT NULL,
    "PityLimit" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "StartAt" timestamp with time zone NOT NULL,
    "EndAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."GachaBanners" OWNER TO postgres;

--
-- Name: GachaPullHistories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."GachaPullHistories" (
    "Id" uuid NOT NULL,
    "PlayerProfileId" uuid NOT NULL,
    "GachaBannerId" uuid NOT NULL,
    "RewardItemId" uuid NOT NULL,
    "PullCount" integer NOT NULL,
    "CostSpent" numeric NOT NULL,
    "PulledAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."GachaPullHistories" OWNER TO postgres;

--
-- Name: InventoryItems; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."InventoryItems" (
    "Id" uuid NOT NULL,
    "PlayerProfileId" uuid NOT NULL,
    "ItemId" uuid NOT NULL,
    "Quantity" integer NOT NULL,
    "IsEquipped" boolean NOT NULL,
    "EnhancementLevel" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."InventoryItems" OWNER TO postgres;

--
-- Name: Items; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Items" (
    "Id" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Description" character varying(1000),
    "Type" integer NOT NULL,
    "Rarity" integer NOT NULL,
    "Slot" integer NOT NULL,
    "BaseValue" numeric NOT NULL,
    "MaxStack" integer NOT NULL,
    "IsTradable" boolean NOT NULL,
    "IsActive" boolean NOT NULL,
    "IconUrl" text,
    "CreatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."Items" OWNER TO postgres;

--
-- Name: Mails; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Mails" (
    "Id" uuid NOT NULL,
    "PlayerProfileId" uuid NOT NULL,
    "Title" character varying(200) NOT NULL,
    "Content" text NOT NULL,
    "Type" integer NOT NULL,
    "AttachedGold" numeric NOT NULL,
    "AttachedGems" numeric NOT NULL,
    "AttachedItemId" uuid,
    "AttachedItemQuantity" integer NOT NULL,
    "IsRead" boolean NOT NULL,
    "IsClaimed" boolean NOT NULL,
    "SentAt" timestamp with time zone NOT NULL,
    "ExpiredAt" timestamp with time zone
);


ALTER TABLE public."Mails" OWNER TO postgres;

--
-- Name: Monsters; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Monsters" (
    "Id" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Type" integer NOT NULL,
    "Level" integer NOT NULL,
    "Health" integer NOT NULL,
    "Attack" integer NOT NULL,
    "Defense" integer NOT NULL,
    "ExperienceReward" integer NOT NULL,
    "GoldReward" numeric NOT NULL,
    "IsActive" boolean NOT NULL
);


ALTER TABLE public."Monsters" OWNER TO postgres;

--
-- Name: PlayerCurrencyLogs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."PlayerCurrencyLogs" (
    "Id" uuid NOT NULL,
    "PlayerProfileId" uuid NOT NULL,
    "Currency" integer NOT NULL,
    "Type" integer NOT NULL,
    "Amount" numeric NOT NULL,
    "BalanceAfter" numeric NOT NULL,
    "Note" text,
    "CreatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."PlayerCurrencyLogs" OWNER TO postgres;

--
-- Name: PlayerProfiles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."PlayerProfiles" (
    "Id" uuid NOT NULL,
    "AccountId" uuid NOT NULL,
    "DisplayName" character varying(100) NOT NULL,
    "AvatarUrl" text NOT NULL,
    "Class" integer NOT NULL,
    "Level" integer NOT NULL,
    "ExperiencePoints" integer NOT NULL,
    "Gold" numeric NOT NULL,
    "Gems" numeric NOT NULL,
    "Energy" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."PlayerProfiles" OWNER TO postgres;

--
-- Name: PlayerQuests; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."PlayerQuests" (
    "Id" uuid NOT NULL,
    "PlayerProfileId" uuid NOT NULL,
    "QuestId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "Progress" integer NOT NULL,
    "TargetValue" integer NOT NULL,
    "AcceptedAt" timestamp with time zone NOT NULL,
    "CompletedAt" timestamp with time zone,
    "ClaimedAt" timestamp with time zone
);


ALTER TABLE public."PlayerQuests" OWNER TO postgres;

--
-- Name: PlayerSkills; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."PlayerSkills" (
    "Id" uuid NOT NULL,
    "PlayerProfileId" uuid NOT NULL,
    "SkillId" uuid NOT NULL,
    "Level" integer NOT NULL,
    "Experience" integer NOT NULL,
    "IsEquipped" boolean NOT NULL,
    "UnlockedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."PlayerSkills" OWNER TO postgres;

--
-- Name: PlayerStats; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."PlayerStats" (
    "Id" uuid NOT NULL,
    "PlayerProfileId" uuid NOT NULL,
    "Health" integer NOT NULL,
    "Mana" integer NOT NULL,
    "Strength" integer NOT NULL,
    "Defense" integer NOT NULL,
    "Agility" integer NOT NULL,
    "Intelligence" integer NOT NULL,
    "Endurance" integer NOT NULL,
    "Luck" integer NOT NULL,
    "CriticalRate" integer NOT NULL,
    "CriticalDamage" integer NOT NULL,
    "ArmorPenetration" integer NOT NULL,
    "SkillPoints" integer NOT NULL,
    "TotalWins" integer NOT NULL,
    "TotalLosses" integer NOT NULL,
    "TotalKills" integer NOT NULL,
    "TotalDeaths" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."PlayerStats" OWNER TO postgres;

--
-- Name: PurchaseHistories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."PurchaseHistories" (
    "Id" uuid NOT NULL,
    "PlayerProfileId" uuid NOT NULL,
    "ShopItemId" uuid NOT NULL,
    "Quantity" integer NOT NULL,
    "TotalPrice" numeric NOT NULL,
    "PurchasedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."PurchaseHistories" OWNER TO postgres;

--
-- Name: Quests; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Quests" (
    "Id" uuid NOT NULL,
    "Title" character varying(200) NOT NULL,
    "Description" text,
    "Type" integer NOT NULL,
    "DefaultStatus" integer NOT NULL,
    "RequiredLevel" integer NOT NULL,
    "RewardExperience" integer NOT NULL,
    "RewardGold" numeric NOT NULL,
    "RewardGems" numeric NOT NULL,
    "RewardItemId" uuid,
    "IsActive" boolean NOT NULL
);


ALTER TABLE public."Quests" OWNER TO postgres;

--
-- Name: ShopItems; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ShopItems" (
    "Id" uuid NOT NULL,
    "ItemId" uuid NOT NULL,
    "Currency" integer NOT NULL,
    "Price" numeric NOT NULL,
    "Stock" integer NOT NULL,
    "DailyPurchaseLimit" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "AvailableFrom" timestamp with time zone,
    "AvailableTo" timestamp with time zone
);


ALTER TABLE public."ShopItems" OWNER TO postgres;

--
-- Name: Skills; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Skills" (
    "Id" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Description" text,
    "Type" integer NOT NULL,
    "DamageType" integer NOT NULL,
    "TargetType" integer NOT NULL,
    "ClassRequirement" integer NOT NULL,
    "ManaCost" integer NOT NULL,
    "CooldownSeconds" integer NOT NULL,
    "BaseDamage" integer NOT NULL,
    "UnlockLevel" integer NOT NULL,
    "IsActive" boolean NOT NULL
);


ALTER TABLE public."Skills" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: Accounts PK_Accounts; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Accounts"
    ADD CONSTRAINT "PK_Accounts" PRIMARY KEY ("Id");


--
-- Name: Bosses PK_Bosses; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bosses"
    ADD CONSTRAINT "PK_Bosses" PRIMARY KEY ("Id");


--
-- Name: EquipmentStats PK_EquipmentStats; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."EquipmentStats"
    ADD CONSTRAINT "PK_EquipmentStats" PRIMARY KEY ("Id");


--
-- Name: Friends PK_Friends; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Friends"
    ADD CONSTRAINT "PK_Friends" PRIMARY KEY ("Id");


--
-- Name: GachaBannerItems PK_GachaBannerItems; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GachaBannerItems"
    ADD CONSTRAINT "PK_GachaBannerItems" PRIMARY KEY ("Id");


--
-- Name: GachaBanners PK_GachaBanners; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GachaBanners"
    ADD CONSTRAINT "PK_GachaBanners" PRIMARY KEY ("Id");


--
-- Name: GachaPullHistories PK_GachaPullHistories; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GachaPullHistories"
    ADD CONSTRAINT "PK_GachaPullHistories" PRIMARY KEY ("Id");


--
-- Name: InventoryItems PK_InventoryItems; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."InventoryItems"
    ADD CONSTRAINT "PK_InventoryItems" PRIMARY KEY ("Id");


--
-- Name: Items PK_Items; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Items"
    ADD CONSTRAINT "PK_Items" PRIMARY KEY ("Id");


--
-- Name: Mails PK_Mails; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Mails"
    ADD CONSTRAINT "PK_Mails" PRIMARY KEY ("Id");


--
-- Name: Monsters PK_Monsters; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Monsters"
    ADD CONSTRAINT "PK_Monsters" PRIMARY KEY ("Id");


--
-- Name: PlayerCurrencyLogs PK_PlayerCurrencyLogs; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerCurrencyLogs"
    ADD CONSTRAINT "PK_PlayerCurrencyLogs" PRIMARY KEY ("Id");


--
-- Name: PlayerProfiles PK_PlayerProfiles; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerProfiles"
    ADD CONSTRAINT "PK_PlayerProfiles" PRIMARY KEY ("Id");


--
-- Name: PlayerQuests PK_PlayerQuests; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerQuests"
    ADD CONSTRAINT "PK_PlayerQuests" PRIMARY KEY ("Id");


--
-- Name: PlayerSkills PK_PlayerSkills; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerSkills"
    ADD CONSTRAINT "PK_PlayerSkills" PRIMARY KEY ("Id");


--
-- Name: PlayerStats PK_PlayerStats; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerStats"
    ADD CONSTRAINT "PK_PlayerStats" PRIMARY KEY ("Id");


--
-- Name: PurchaseHistories PK_PurchaseHistories; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PurchaseHistories"
    ADD CONSTRAINT "PK_PurchaseHistories" PRIMARY KEY ("Id");


--
-- Name: Quests PK_Quests; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Quests"
    ADD CONSTRAINT "PK_Quests" PRIMARY KEY ("Id");


--
-- Name: ShopItems PK_ShopItems; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShopItems"
    ADD CONSTRAINT "PK_ShopItems" PRIMARY KEY ("Id");


--
-- Name: Skills PK_Skills; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Skills"
    ADD CONSTRAINT "PK_Skills" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_EquipmentStats_ItemId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_EquipmentStats_ItemId" ON public."EquipmentStats" USING btree ("ItemId");


--
-- Name: IX_Friends_AddresseeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Friends_AddresseeId" ON public."Friends" USING btree ("AddresseeId");


--
-- Name: IX_Friends_RequesterId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Friends_RequesterId" ON public."Friends" USING btree ("RequesterId");


--
-- Name: IX_GachaBannerItems_GachaBannerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_GachaBannerItems_GachaBannerId" ON public."GachaBannerItems" USING btree ("GachaBannerId");


--
-- Name: IX_GachaBannerItems_ItemId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_GachaBannerItems_ItemId" ON public."GachaBannerItems" USING btree ("ItemId");


--
-- Name: IX_GachaBannerItems_ItemId1; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_GachaBannerItems_ItemId1" ON public."GachaBannerItems" USING btree ("ItemId1");


--
-- Name: IX_GachaPullHistories_GachaBannerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_GachaPullHistories_GachaBannerId" ON public."GachaPullHistories" USING btree ("GachaBannerId");


--
-- Name: IX_GachaPullHistories_PlayerProfileId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_GachaPullHistories_PlayerProfileId" ON public."GachaPullHistories" USING btree ("PlayerProfileId");


--
-- Name: IX_GachaPullHistories_RewardItemId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_GachaPullHistories_RewardItemId" ON public."GachaPullHistories" USING btree ("RewardItemId");


--
-- Name: IX_InventoryItems_ItemId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_InventoryItems_ItemId" ON public."InventoryItems" USING btree ("ItemId");


--
-- Name: IX_InventoryItems_PlayerProfileId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_InventoryItems_PlayerProfileId" ON public."InventoryItems" USING btree ("PlayerProfileId");


--
-- Name: IX_Mails_AttachedItemId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Mails_AttachedItemId" ON public."Mails" USING btree ("AttachedItemId");


--
-- Name: IX_Mails_PlayerProfileId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Mails_PlayerProfileId" ON public."Mails" USING btree ("PlayerProfileId");


--
-- Name: IX_PlayerCurrencyLogs_PlayerProfileId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_PlayerCurrencyLogs_PlayerProfileId" ON public."PlayerCurrencyLogs" USING btree ("PlayerProfileId");


--
-- Name: IX_PlayerProfiles_AccountId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_PlayerProfiles_AccountId" ON public."PlayerProfiles" USING btree ("AccountId");


--
-- Name: IX_PlayerQuests_PlayerProfileId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_PlayerQuests_PlayerProfileId" ON public."PlayerQuests" USING btree ("PlayerProfileId");


--
-- Name: IX_PlayerQuests_QuestId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_PlayerQuests_QuestId" ON public."PlayerQuests" USING btree ("QuestId");


--
-- Name: IX_PlayerSkills_PlayerProfileId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_PlayerSkills_PlayerProfileId" ON public."PlayerSkills" USING btree ("PlayerProfileId");


--
-- Name: IX_PlayerSkills_SkillId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_PlayerSkills_SkillId" ON public."PlayerSkills" USING btree ("SkillId");


--
-- Name: IX_PlayerStats_PlayerProfileId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_PlayerStats_PlayerProfileId" ON public."PlayerStats" USING btree ("PlayerProfileId");


--
-- Name: IX_PurchaseHistories_PlayerProfileId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_PurchaseHistories_PlayerProfileId" ON public."PurchaseHistories" USING btree ("PlayerProfileId");


--
-- Name: IX_PurchaseHistories_ShopItemId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_PurchaseHistories_ShopItemId" ON public."PurchaseHistories" USING btree ("ShopItemId");


--
-- Name: IX_Quests_RewardItemId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Quests_RewardItemId" ON public."Quests" USING btree ("RewardItemId");


--
-- Name: IX_ShopItems_ItemId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ShopItems_ItemId" ON public."ShopItems" USING btree ("ItemId");


--
-- Name: EquipmentStats FK_EquipmentStats_Items_ItemId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."EquipmentStats"
    ADD CONSTRAINT "FK_EquipmentStats_Items_ItemId" FOREIGN KEY ("ItemId") REFERENCES public."Items"("Id") ON DELETE CASCADE;


--
-- Name: Friends FK_Friends_PlayerProfiles_AddresseeId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Friends"
    ADD CONSTRAINT "FK_Friends_PlayerProfiles_AddresseeId" FOREIGN KEY ("AddresseeId") REFERENCES public."PlayerProfiles"("Id") ON DELETE RESTRICT;


--
-- Name: Friends FK_Friends_PlayerProfiles_RequesterId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Friends"
    ADD CONSTRAINT "FK_Friends_PlayerProfiles_RequesterId" FOREIGN KEY ("RequesterId") REFERENCES public."PlayerProfiles"("Id") ON DELETE RESTRICT;


--
-- Name: GachaBannerItems FK_GachaBannerItems_GachaBanners_GachaBannerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GachaBannerItems"
    ADD CONSTRAINT "FK_GachaBannerItems_GachaBanners_GachaBannerId" FOREIGN KEY ("GachaBannerId") REFERENCES public."GachaBanners"("Id") ON DELETE CASCADE;


--
-- Name: GachaBannerItems FK_GachaBannerItems_Items_ItemId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GachaBannerItems"
    ADD CONSTRAINT "FK_GachaBannerItems_Items_ItemId" FOREIGN KEY ("ItemId") REFERENCES public."Items"("Id") ON DELETE RESTRICT;


--
-- Name: GachaBannerItems FK_GachaBannerItems_Items_ItemId1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GachaBannerItems"
    ADD CONSTRAINT "FK_GachaBannerItems_Items_ItemId1" FOREIGN KEY ("ItemId1") REFERENCES public."Items"("Id");


--
-- Name: GachaPullHistories FK_GachaPullHistories_GachaBanners_GachaBannerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GachaPullHistories"
    ADD CONSTRAINT "FK_GachaPullHistories_GachaBanners_GachaBannerId" FOREIGN KEY ("GachaBannerId") REFERENCES public."GachaBanners"("Id") ON DELETE RESTRICT;


--
-- Name: GachaPullHistories FK_GachaPullHistories_Items_RewardItemId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GachaPullHistories"
    ADD CONSTRAINT "FK_GachaPullHistories_Items_RewardItemId" FOREIGN KEY ("RewardItemId") REFERENCES public."Items"("Id") ON DELETE RESTRICT;


--
-- Name: GachaPullHistories FK_GachaPullHistories_PlayerProfiles_PlayerProfileId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GachaPullHistories"
    ADD CONSTRAINT "FK_GachaPullHistories_PlayerProfiles_PlayerProfileId" FOREIGN KEY ("PlayerProfileId") REFERENCES public."PlayerProfiles"("Id") ON DELETE CASCADE;


--
-- Name: InventoryItems FK_InventoryItems_Items_ItemId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."InventoryItems"
    ADD CONSTRAINT "FK_InventoryItems_Items_ItemId" FOREIGN KEY ("ItemId") REFERENCES public."Items"("Id") ON DELETE RESTRICT;


--
-- Name: InventoryItems FK_InventoryItems_PlayerProfiles_PlayerProfileId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."InventoryItems"
    ADD CONSTRAINT "FK_InventoryItems_PlayerProfiles_PlayerProfileId" FOREIGN KEY ("PlayerProfileId") REFERENCES public."PlayerProfiles"("Id") ON DELETE CASCADE;


--
-- Name: Mails FK_Mails_Items_AttachedItemId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Mails"
    ADD CONSTRAINT "FK_Mails_Items_AttachedItemId" FOREIGN KEY ("AttachedItemId") REFERENCES public."Items"("Id") ON DELETE RESTRICT;


--
-- Name: Mails FK_Mails_PlayerProfiles_PlayerProfileId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Mails"
    ADD CONSTRAINT "FK_Mails_PlayerProfiles_PlayerProfileId" FOREIGN KEY ("PlayerProfileId") REFERENCES public."PlayerProfiles"("Id") ON DELETE CASCADE;


--
-- Name: PlayerCurrencyLogs FK_PlayerCurrencyLogs_PlayerProfiles_PlayerProfileId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerCurrencyLogs"
    ADD CONSTRAINT "FK_PlayerCurrencyLogs_PlayerProfiles_PlayerProfileId" FOREIGN KEY ("PlayerProfileId") REFERENCES public."PlayerProfiles"("Id") ON DELETE CASCADE;


--
-- Name: PlayerProfiles FK_PlayerProfiles_Accounts_AccountId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerProfiles"
    ADD CONSTRAINT "FK_PlayerProfiles_Accounts_AccountId" FOREIGN KEY ("AccountId") REFERENCES public."Accounts"("Id") ON DELETE CASCADE;


--
-- Name: PlayerQuests FK_PlayerQuests_PlayerProfiles_PlayerProfileId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerQuests"
    ADD CONSTRAINT "FK_PlayerQuests_PlayerProfiles_PlayerProfileId" FOREIGN KEY ("PlayerProfileId") REFERENCES public."PlayerProfiles"("Id") ON DELETE CASCADE;


--
-- Name: PlayerQuests FK_PlayerQuests_Quests_QuestId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerQuests"
    ADD CONSTRAINT "FK_PlayerQuests_Quests_QuestId" FOREIGN KEY ("QuestId") REFERENCES public."Quests"("Id") ON DELETE RESTRICT;


--
-- Name: PlayerSkills FK_PlayerSkills_PlayerProfiles_PlayerProfileId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerSkills"
    ADD CONSTRAINT "FK_PlayerSkills_PlayerProfiles_PlayerProfileId" FOREIGN KEY ("PlayerProfileId") REFERENCES public."PlayerProfiles"("Id") ON DELETE CASCADE;


--
-- Name: PlayerSkills FK_PlayerSkills_Skills_SkillId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerSkills"
    ADD CONSTRAINT "FK_PlayerSkills_Skills_SkillId" FOREIGN KEY ("SkillId") REFERENCES public."Skills"("Id") ON DELETE RESTRICT;


--
-- Name: PlayerStats FK_PlayerStats_PlayerProfiles_PlayerProfileId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PlayerStats"
    ADD CONSTRAINT "FK_PlayerStats_PlayerProfiles_PlayerProfileId" FOREIGN KEY ("PlayerProfileId") REFERENCES public."PlayerProfiles"("Id") ON DELETE CASCADE;


--
-- Name: PurchaseHistories FK_PurchaseHistories_PlayerProfiles_PlayerProfileId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PurchaseHistories"
    ADD CONSTRAINT "FK_PurchaseHistories_PlayerProfiles_PlayerProfileId" FOREIGN KEY ("PlayerProfileId") REFERENCES public."PlayerProfiles"("Id") ON DELETE CASCADE;


--
-- Name: PurchaseHistories FK_PurchaseHistories_ShopItems_ShopItemId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PurchaseHistories"
    ADD CONSTRAINT "FK_PurchaseHistories_ShopItems_ShopItemId" FOREIGN KEY ("ShopItemId") REFERENCES public."ShopItems"("Id") ON DELETE RESTRICT;


--
-- Name: Quests FK_Quests_Items_RewardItemId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Quests"
    ADD CONSTRAINT "FK_Quests_Items_RewardItemId" FOREIGN KEY ("RewardItemId") REFERENCES public."Items"("Id") ON DELETE RESTRICT;


--
-- Name: ShopItems FK_ShopItems_Items_ItemId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShopItems"
    ADD CONSTRAINT "FK_ShopItems_Items_ItemId" FOREIGN KEY ("ItemId") REFERENCES public."Items"("Id") ON DELETE RESTRICT;


--
-- PostgreSQL database dump complete
--

\unrestrict asudw8LNi6NMjXxNW8mpWzE61qd9h6Po9UDBy3IkwFLODZk8pqKuVaeX1nZSM5y

