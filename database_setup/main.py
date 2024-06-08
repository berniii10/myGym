import os, json
import psycopg2
from dotenv import load_dotenv

code_path = "database_setup"

class Exercise:
    def __init__(self, name, force, level, mechanic, equipment, primary_muscles, secondary_muscles, instructions, category, images_url) -> None:
        self.name = name
        self.force = force
        self.level = level
        self.mechanic = mechanic
        self.equipment = equipment
        self.primary_muscles = primary_muscles
        self.secondary_muscles = secondary_muscles
        self.instructions = instructions
        self.category = category
        self.images_url = images_url

def connectToDb():
    try:
        load_dotenv()
        with open(os.path.join(code_path, 'db_config.json'), 'r') as file:
            config = json.load(file)
        
        db_config = {
            'user': os.getenv('DB_USER'),
            'password': os.getenv('DB_PASSWORD'),
            'host': config['host'],
            'database': config['database'],
            'port': config['port'],
            'connect_timeout': config['connect_timeout']
        }
        myDb = psycopg2.connect(**db_config)
        
        if myDb is None:
             print("Failed to connect to DB")
        else:
            print("Success connecting to DB")
            return myDb, myDb.cursor()
        
    except psycopg2.Error as e:
        print(f"Error: {e}")    

def closeConnection(myDb, myDb_cursor):
    myDb_cursor.close()
    myDb.close()

def addExercise(exercise, myDb_cursor):
    try:
        # Check if the exercise with the same name already exists
        myDb_cursor.execute("""
            SELECT id FROM Exercises WHERE name = %s
        """, (exercise.name,))
        existing_exercise = myDb_cursor.fetchone()

        if existing_exercise is not None:
            print(f"Exercise with the name '{exercise.name}' already exists with ID: {existing_exercise[0]}")
            return existing_exercise[0]

        # Step 1: Insert into Exercises table and return the new exercise_id
        myDb_cursor.execute("""
            INSERT INTO Exercises (name, force, level, mechanic, equipment, category, image_url)
            VALUES (%s, %s, %s, %s, %s, %s, %s)
            RETURNING id
        """, (exercise.name, exercise.force, exercise.level, exercise.mechanic, exercise.equipment, exercise.category, exercise.images_url))
        
        exercise_id = myDb_cursor.fetchone()[0]

        # Add the muscle id for primary
        for muscle in exercise.primary_muscles:
            myDb_cursor.execute("""
                SELECT id from muscles where name = %s
            """, (muscle,))  # Assuming primaryMuscles list has at least one muscle
            
            muscle_id = myDb_cursor.fetchone()

            myDb_cursor.execute("""
                INSERT INTO ExerciseMuscles (exercise_id, muscle_id, type)
                VALUES (%s, %s, %s)
            """, (exercise_id, muscle_id, 'primary'))

        # Add the muscle id for secondary
        for muscle in exercise.secondary_muscles:
            myDb_cursor.execute("""
                SELECT id from muscles where name = %s
            """, (muscle,))  # Assuming primaryMuscles list has at least one muscle
            
            muscle_id = myDb_cursor.fetchone()
            myDb_cursor.execute("""
                INSERT INTO ExerciseMuscles (exercise_id, muscle_id, type)
                VALUES (%s, %s, %s)
            """, (exercise_id, muscle_id, 'secondary'))

        # Step 5: Insert into Instructions table
        instructions = exercise.instructions
        for step_number, description in enumerate(instructions, 1):
            myDb_cursor.execute("""
                INSERT INTO Instructions (exercise_id, step_number, description)
                VALUES (%s, %s, %s)
            """, (exercise_id, step_number, description))

        return exercise_id

    except Exception as e:
        # Rollback the transaction if any error occurs
        # myDb_cursor.rollback()
        print(f"An error occurred: {e}")
        return -1

def addMuscle(muscle, cur):
    cur.execute("""
        SELECT id FROM muscles WHERE name = %s
    """, (muscle,))
    existing_exercise = cur.fetchone()

    if existing_exercise == None:
        cur.execute("""
            INSERT INTO muscles (name)
            VALUES (%s)
        """, (muscle,))
        print("Muscle added")
    else:
        print("Muscle already exists")

def dbSetUp(myDb_connection, myDb_cursor):
    exercises = []
    muscles = []
    
    print("Loading Exercises")
    for dir_name in os.listdir(os.path.join(code_path, "exercises")):
        # print(dir_name)
        # print(os.getcwd())
        with open(os.path.join(code_path, 'exercises', dir_name, 'exercise.json'), 'r', encoding='utf-8') as file:
            ex = json.load(file)
        
        exercise = Exercise(ex['name'], ex['force'], ex['level'], ex['mechanic'], ex['equipment'], ex['primaryMuscles'], ex['secondaryMuscles'], ex['instructions'], ex['category'], os.path.join('exercises_images', dir_name))
        exercises.append(exercise)      

        for muscle in muscles:
            if muscle not in muscles:
                muscles.append(exercise.primary_muscles)
        for muscle in exercise.secondary_muscles:
            if muscle not in muscles:
                muscles.append(muscle)
    
    print("Inserting muscles into DB")
    for muscle in muscles:
        if addMuscle(muscle, myDb_cursor) == -1:
            return -1
        myDb_connection.commit()

    print("Inserting exercises into DB")
    for i, exercise in enumerate(exercises):
        if addExercise(exercise, myDb_cursor) == -1:
            return -1
        myDb_connection.commit()

    return 1

def myMain():
    myDb_connection, myDb_cursor = connectToDb()
    if dbSetUp(myDb_connection, myDb_cursor) == 1:
        print("Database loaded correctly")

if __name__ == "__main__":
    myMain()