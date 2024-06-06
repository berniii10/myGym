import os, json
import psycopg2
from dotenv import load_dotenv

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
        with open('db_config.json', 'r') as file:
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
    myDb.close()

def dbSetUp():
    exercises = []
    muscles = []
    
    for dir_name in os.listdir("exercises"):
        print(dir_name)
        # print(os.getcwd())
        with open(os.path.join('exercises', dir_name, 'exercise.json'), 'r', encoding='utf-8') as file:
            ex = json.load(file)
        
        exercise = Exercise(ex['name'], ex['force'], ex['level'], ex['mechanic'], ex['equipment'], ex['primaryMuscles'], ex['secondaryMuscles'], ex['instructions'], ex['category'], "a")
        exercises.append(exercise)

        if exercise.primary_muscles not in muscles:
            muscles.append(exercise.primary_muscles)
        for muscle in exercise.secondary_muscles:
            if muscle not in muscles:
                muscles.append(muscle)        

    # Muscle table first

def myMain():
    myDb, myDb_cursor = connectToDb()
    dbSetUp()

if __name__ == "__main__":
    myMain()