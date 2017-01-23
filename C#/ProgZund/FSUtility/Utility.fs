namespace FSUtility

type Display() = 
    static member public MAX_DIGITS = 5

type Machine()  =
    
    static let currentDirectory = System.IO.Directory.GetCurrentDirectory()
    static let parent = System.IO.Directory.GetParent(System.IO.Directory.GetParent(currentDirectory).ToString()).ToString();
    static let fullPath f = parent + f
    static let readFile s = System.IO.File.ReadAllText(fullPath(s)) 

    static member public TOTAL_WIDTH = 47.6
    static member public TOTAL_HEIGHT = 32.2
    static member public POINTS_PER_INCH = 2540.0
    static member public INSIDE_KNIFE_POINTS_REMOVAL = 15
    static member public NUMBER_OF_PENS = 4
    static member public FILE_BEGINNING = readFile @"\BEGIN.txt"
    static member public FILE_ENDING = readFile  @"\END.txt"
    static member public PORT = readFile @"\PORT.txt"
        

    


    
    
        
