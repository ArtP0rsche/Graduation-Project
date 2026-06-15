-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: russian_work
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Dumping data for table `event`
--

LOCK TABLES `event` WRITE;
/*!40000 ALTER TABLE `event` DISABLE KEYS */;
INSERT INTO `event` VALUES (1,NULL,'Профориентационный семинар для молодежи','Приглашаем молодых людей на наш профориентационный семинар, где вы сможете определить свои профессиональные предпочтения, получить консультации и составить план карьерного развития. Сделайте первый шаг к своей мечте!',0,'2026-05-12 00:00:00','2026-05-06','Проведено'),(2,NULL,'Мастер-класс по созданию бизнес-плана','Приглашаем вас на мастер-класс, где вы научитесь разрабатывать эффективные бизнес-планы, узнаете о ключевых аспектах предпринимательства и получите практические навыки для запуска собственного дела.',0,'2025-12-14 15:00:00','2025-12-09','Проведено'),(3,NULL,'Тренинг по развитию коммуникативных навыков','Приглашаем вас на тренинг, который поможет вам улучшить навыки общения, работы в команде и уверенного взаимодействия с окружающими. Эти навыки необходимы для успешной карьеры и личностного роста.',0,'2025-11-28 16:15:00','2026-05-05','Отменено'),(4,NULL,'Семинар по развитию лидерских качеств','Приглашаем вас на семинар, который поможет раскрыть ваши лидерские способности, научит управлять командой и достигать поставленных целей. Сделайте шаг к профессиональному росту!',0,'2025-12-08 15:30:00','2025-12-09','Проведено'),(16,5,'Ярмарка вакансий','Пример',20,'2026-05-11 00:00:00','2026-05-13','В планах'),(19,6,'Test Event','Test description',25,'2026-06-12 00:00:00','2026-06-05','В планах');
/*!40000 ALTER TABLE `event` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `role`
--

LOCK TABLES `role` WRITE;
/*!40000 ALTER TABLE `role` DISABLE KEYS */;
INSERT INTO `role` VALUES (1,'Пользователь'),(2,'Менеджер'),(3,'Администратор');
/*!40000 ALTER TABLE `role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,3,'admin','12345678','Артём','Поршнев','Николаевич'),(2,2,'manager','87654321','Светлана','Наумова','Александровна'),(10,1,'imStydent','11111111','Артём','Мошников','Вячеславович'),(11,1,'DenobBlack','22222222','Егор','Баранов','Алексеевич');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-06-15 18:00:05
