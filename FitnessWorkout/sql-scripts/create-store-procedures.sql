CREATE PROCEDURE InsertWorkout(
IN p_Name VARCHAR(255),
IN p_Description TEXT,
IN p_DurationMinutes INT,
IN p_Difficulty ENUM('Easy', 'Medium', 'Hard'),
IN p_CreatedBy INT
)
BEGIN
INSERT INTO Workouts (Name, Description, DurationMinutes, Difficulty, CreatedBy, CreatedAt)
VALUES (p_Name, p_Description, p_DurationMinutes, p_Difficulty, p_CreatedBy, NOW());
SELECT LAST_INSERT_ID() AS Id;
END;


CREATE PROCEDURE InsertMovement(
IN p_Name VARCHAR(255),
IN p_Description TEXT,
IN p_WorkoutId INT,
IN p_CreatedBy INT
)
BEGIN
INSERT INTO Movements (Name, Description, WorkoutId, CreatedBy, CreatedAt)
VALUES (p_Name, p_Description, p_WorkoutId, p_CreatedBy, NOW());
END;


CREATE PROCEDURE InsertWorkoutBodyRegion(
IN p_WorkoutId INT,
IN p_BodyRegionId INT
)
BEGIN
INSERT INTO WorkoutBodyRegions (WorkoutId, BodyRegionId)
VALUES (p_WorkoutId, p_BodyRegionId);
END;



CREATE PROCEDURE GetFilteredWorkouts(
IN p_Duration INT,
IN p_Difficulty ENUM('Easy', 'Medium', 'Hard'),
IN p_BodyRegion VARCHAR(50),
IN p_UseAndFilter BOOLEAN
)
BEGIN
SELECT DISTINCT w.
FROM Workouts w
LEFT JOIN WorkoutBodyRegions wbr ON w.Id = wbr.WorkoutId
LEFT JOIN BodyRegions br ON wbr.BodyRegionId = br.Id
WHERE
(p_UseAndFilter = 1 AND
(p_Duration IS NULL OR w.DurationMinutes = p_Duration) AND
(p_Difficulty IS NULL OR w.Difficulty = p_Difficulty) AND
(p_BodyRegion IS NULL OR br.Name = p_BodyRegion))
OR
(p_UseAndFilter = 0 AND
(p_Duration IS NULL OR w.DurationMinutes = p_Duration) OR
(p_Difficulty IS NULL OR w.Difficulty = p_Difficulty) OR
(p_BodyRegion IS NULL OR br.Name = p_BodyRegion));
END;