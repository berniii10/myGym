from database.common import *

def myMain():
    myDb, myDb_cursor = connectToDb()
    dbSetUp()

if __name__ == "__main__":
    myMain()