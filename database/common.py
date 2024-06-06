import os, json
import psycopg2
from dotenv import load_dotenv
from datastructures.exercise import Exercise

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
            return myDb
        
    except psycopg2.Error as e:
        print(f"Error: {e}")    

def dbSetUp():
    exercises = []
    
    for dir_name in os.listdir("exercises"):
        print(dir_name)
        # print(os.getcwd())
        with open(os.path.join('exercises', dir_name, 'exercise.json'), 'r', encoding='utf-8') as file:
            ex = json.load(file)
        
        exercise = Exercise(ex['name'], ex['force'], ex['level'], ex['mechanic'], ex['equipment'], ex['primaryMuscles'], ex['secondaryMuscles'], ex['instructions'], ex['category'], "a")
        exercises.append(exercise)

    # Muscle table first