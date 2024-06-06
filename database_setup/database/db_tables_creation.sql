CREATE TABLE Exercises (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    force TEXT NOT NULL,
    level TEXT NOT NULL,
    mechanic TEXT NOT NULL,
    equipment TEXT NOT NULL,
    category TEXT NOT NULL,
    image_url TEXT
);

CREATE TABLE Muscles (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL
);

CREATE TABLE ExerciseMuscles (
    id SERIAL PRIMARY KEY,
    exercise_id INTEGER NOT NULL REFERENCES Exercises(id),
    muscle_id INTEGER NOT NULL REFERENCES Muscles(id),
    type TEXT NOT NULL,
    UNIQUE(exercise_id, muscle_id, type)
);

CREATE TABLE Instructions (
    id SERIAL PRIMARY KEY,
    exercise_id INTEGER NOT NULL REFERENCES Exercises(id),
    step_number INTEGER NOT NULL,
    description TEXT NOT NULL
);

CREATE TABLE ExerciseImages (
    id SERIAL PRIMARY KEY,
    exercise_id INTEGER NOT NULL REFERENCES Exercises(id),
    image_url TEXT NOT NULL
);
