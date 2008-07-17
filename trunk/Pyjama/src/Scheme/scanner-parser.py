# includes support for vectors and rationals

import string

#-----------------------------------------------------------------------
# scanner - character stream represented as a position number

charsToScan = None

def first(n):
    return charsToScan[n]

def remaining(n):
    return n + 1

def scanInput(input):
    global charsToScan
    charsToScan = input + "\0"
    chars = 0
    tokens = []
    while True:
        token, charsLeft = applyAction(("goto", "start-state"), [], chars)
        tokens.append(token)
        if isTokenType(token, "end-marker"):
            return tokens
        else:
            chars = charsLeft

#-----------------------------------------------------------------------
# scanner actions

# <action> ::= ("shift", <next-action>)
#            | ("replace", <new-char>, <next-action>)
#            | ("drop", <next-action>)
#            | ("goto", <state>)
#            | ("emit", <token-type>)

def applyAction(action, buffer, chars):
    while True:
        tag = action[0]
        if tag == "shift":
            next = action[1]
            buffer.append(first(chars))
            chars = remaining(chars)
            action = next
        elif tag == "replace":
            newChar, next = action[1], action[2]
            buffer.append(newChar)
            chars = remaining(chars)
            action = next
        elif tag == "drop":
            next = action[1]
            chars = remaining(chars)
            action = next
        elif tag == "goto":
            state = action[1]
            action = applyState(state, first(chars))
        elif tag == "emit":
            tokenType = action[1]
            token = convertBufferToToken(tokenType, buffer)
            return token, chars
        else:
            raise Exception("invalid action: %s in applyAction" % action)

def scanError(c):
    if c == '\0':
        raise Exception("unexpected end of input in scan")
    else:
        raise Exception("unexpected character %s encountered" % c)

# changed
def convertBufferToToken(tokenType, buffer):
    datum = string.join(buffer, '')
    if tokenType == "integer":
        return ("integer", datum)
    elif tokenType == "decimal":
        return ("decimal", datum)
    elif tokenType == "rational":
        numerator, denominator = string.split(datum, '/')
        return ("rational", numerator, denominator)
    elif tokenType == "identifier":
        return ("identifier", datum)
    elif tokenType == "boolean":
        return ("boolean", datum == "t" or datum == "T")
    elif tokenType == "character":
        return ("character", datum[0])
    elif tokenType == "named-character":
        if datum == "nul": return ("character", '\0')
        if datum == "space": return ("character", ' ')
        if datum == "tab": return ("character", '\t')
        if datum == "newline": return ("character", '\n')
        if datum == "linefeed": return ("character", '\n')
        if datum == "backspace": return ("character", '\b')
        if datum == "return": return ("character", '\r')
        if datum == "page": return ("character", '\f')
        raise Exception("invalid character name #\\%s" % datum)
    elif tokenType == "string":
        return ("string", datum)
    else:
        return (tokenType,)

def isTokenType(token, tokenType):
    return token[0] == tokenType

#-----------------------------------------------------------------------
# character categories

alphabeticChars = string.letters

numericChars = string.digits

whitespaceChars = string.whitespace

delimiterChars = whitespaceChars + '()[]";#\0'

initialChars = alphabeticChars + '!$%&*/:<=>?^_~'

specialSubsequentChars = '+-@.'

subsequentChars = initialChars + numericChars + specialSubsequentChars

signChars = '+-'

booleanChars = 'tTfF'

#-----------------------------------------------------------------------
# finite state automaton

# this function is just a big lookup table
def applyState(state, c):
    if state == "start-state":
        if c in whitespaceChars: return ("drop", ("goto", "start-state"))
        if c == ';': return ("drop", ("goto", "comment-state"))
        if c == '(': return ("drop", ("emit", "lparen"))
        if c == '[': return ("drop", ("emit", "lbracket"))
        if c == ')': return ("drop", ("emit", "rparen"))
        if c == ']': return ("drop", ("emit", "rbracket"))
        if c == "'": return ("drop", ("emit", "apostrophe"))
        if c == '#': return ("drop", ("goto", "hash-prefix-state"))
        if c == '"': return ("drop", ("goto", "string-state"))
        if c in initialChars: return ("shift", ("goto", "identifier-state"))
        if c in signChars: return ("shift", ("goto", "signed-state"))
        if c == '.': return ("shift", ("goto", "decimal-point-state"))
        if c in numericChars: return ("shift", ("goto", "whole-number-state"))
        if c == '\0': return ("emit", "end-marker")
        else: scanError(c)
    elif state == "comment-state":
        if c == '\n': return ("drop", ("goto", "start-state"))
        if c == '\0': return ("goto", "start-state")
        else: return ("drop", ("goto", "comment-state"))
    elif state == "hash-prefix-state":
        if c in booleanChars: return ("shift", ("emit", "boolean"))
        if c == '\\': return ("drop", ("goto", "character-state"))
        # new
        if c == '(': return ("drop", ("emit", "lvector"))
        else: scanError(c)
    elif state == "character-state":
        if c in alphabeticChars: return ("shift", ("goto", "alphabetic-character-state"))
        if c != '\0': return ("shift", ("emit", "character"))
        else: scanError(c)
    elif state == "alphabetic-character-state":
        if c in alphabeticChars: return ("shift", ("goto", "named-character-state"))
        else: return ("emit", "character")
    elif state == "named-character-state":
        if c in delimiterChars: return ("emit", "named-character")
        else: return ("shift", ("goto", "named-character-state"))
    elif state == "string-state":
        if c == '"': return ("drop", ("emit", "string"))
        if c == '\\': return ("drop", ("goto", "string-escape-state"))
        if c == '\0': scanError(c)
        else: return ("shift", ("goto", "string-state"))
    elif state == "string-escape-state":
        if c == '"': return ("shift", ("goto", "string-state"))
        if c == '\\': return ("shift", ("goto", "string-state"))
        if c == 'b': return ("replace", '\b', ("goto", "string-state"))
        if c == 'f': return ("replace", '\f', ("goto", "string-state"))
        if c == 'n': return ("replace", '\n', ("goto", "string-state"))
        if c == 't': return ("replace", '\t', ("goto", "string-state"))
        if c == 'r': return ("replace", '\r', ("goto", "string-state"))
        else: scanError(c)
    elif state == "identifier-state":
        if c in subsequentChars: return ("shift", ("goto", "identifier-state"))
        if c in delimiterChars: return ("emit", "identifier")
        else: scanError(c)
    elif state == "signed-state":
        if c in numericChars: return ("shift", ("goto", "whole-number-state"))
        if c == '.': return ("shift", ("goto", "signed-decimal-point-state"))
        if c in delimiterChars: return ("emit", "identifier")
        # changed
        if c in subsequentChars: return ("shift", ("goto", "identifier-state"))
        else: scanError(c)
    elif state == "decimal-point-state":
        if c in numericChars: return ("shift", ("goto", "fractional-number-state"))
        if c in delimiterChars: return ("emit", "dot")
        # changed
        if c in subsequentChars: return ("shift", ("goto", "identifier-state"))
        else: scanError(c)
    elif state == "signed-decimal-point-state":
        if c in numericChars: return ("shift", ("goto", "fractional-number-state"))
        if c in delimiterChars: return ("emit", "identifier")
        # changed
        if c in subsequentChars: return ("shift", ("goto", "identifier-state"))
        else: scanError(c)
    elif state == "whole-number-state":
        if c in numericChars: return ("shift", ("goto", "whole-number-state"))
        if c == '.': return ("shift", ("goto", "fractional-number-state"))
        # new
        if c == '/': return ("shift", ("goto", "rational-number-state"))
        # new
        if c == 'e' or c == 'E': return ("shift", ("goto", "suffix-state"))
        # changed
        if c in delimiterChars: return ("emit", "integer")
        # changed
        if c in subsequentChars: return ("shift", ("goto", "identifier-state"))
        else: scanError(c)
    elif state == "fractional-number-state":
        if c in numericChars: return ("shift", ("goto", "fractional-number-state"))
        # new
        if c == 'e' or c == 'E': return ("shift", ("goto", "suffix-state"))
        # changed
        if c in delimiterChars: return ("emit", "decimal")
        # changed
        if c in subsequentChars: return ("shift", ("goto", "identifier-state"))
        else: scanError(c)
    # new
    elif state == "rational-number-state":
        if c in numericChars: return ("shift", ("goto", "rational-number-state*"))
        if c in delimiterChars: return ("emit", "identifier")
        # changed
        if c in subsequentChars: return ("shift", ("goto", "identifier-state"))
        else: scanError(c)
    # new
    elif state == "rational-number-state*":
        if c in numericChars: return ("shift", ("goto", "rational-number-state*"))
        if c in delimiterChars: return ("emit", "rational")
        # changed
        if c in subsequentChars: return ("shift", ("goto", "identifier-state"))
        else: scanError(c)
    # new
    elif state == "suffix-state":
        if c in signChars: return ("shift", ("goto", "signed-exponent-state"))
        if c in numericChars: return ("shift", ("goto", "exponent-state"))
        if c in delimiterChars: return ("emit", "identifier")
        # changed
        if c in subsequentChars: return ("shift", ("goto", "identifier-state"))
        else: scanError(c)
    # new
    elif state == "signed-exponent-state":
        if c in numericChars: return ("shift", ("goto", "exponent-state"))
        if c in delimiterChars: return ("emit", "identifier")
        # changed
        if c in subsequentChars: return ("shift", ("goto", "identifier-state"))
        else: scanError(c)
    # new
    elif state == "exponent-state":
        if c in numericChars: return ("shift", ("goto", "exponent-state"))
        if c in delimiterChars: return ("emit", "decimal")
        # changed
        if c in subsequentChars: return ("shift", ("goto", "identifier-state"))
        else: scanError(c)
    else:
        raise Exception("invalid state %s in applyState" % state)

# scans an entire file and returns a list of all of the tokens
def scanFile(filename):
    f = open(filename)
    content = f.read()
    f.close()
    return scanInput(content)

# example:
# >>> scanFile("scanner-parser.ss")

#-----------------------------------------------------------------------
# parser (registerized)

# global registers
tokens_reg = None
k_reg = None
terminator_reg = None
sexp_reg = None
pc = None

def parse(input):
    global tokens_reg, k_reg, pc
    tokens_reg = scanInput(input)
    k_reg = ("init-cont",)
    pc = parseSexp
    return run()

# the trampoline
def run():
    global pc, sexp_reg
    while pc is not None:
        pc()
    return sexp_reg

def parseSexp():
    global tokens_reg, k_reg, terminator_reg, sexp_reg, pc
    token = tokens_reg[0]
    tag = token[0]
# deleted
#    if tag == "number":
#       ...
    # new
    if tag == "integer":
        value = int(token[1])
        sexp_reg = ExactNumber(value)
        tokens_reg.pop(0)
        pc = applyCont
    # new
    elif tag == "decimal":
        value = float(token[1])
        sexp_reg = InexactNumber(value)
        tokens_reg.pop(0)
        pc = applyCont
    # new
    elif tag == "rational":
        num, den = int(token[1]), int(token[2])
        sexp_reg = ExactNumber(num, den)
        tokens_reg.pop(0)
        pc = applyCont
    elif tag == "boolean":
        bool = token[1]
	sexp_reg = Boolean(bool)
        tokens_reg.pop(0)
	pc = applyCont
    elif tag == "character":
        char = token[1]
	sexp_reg = Character(char)
	tokens_reg.pop(0)
	pc = applyCont
    elif tag == "string":
        str = token[1]
	sexp_reg = String(str)
	tokens_reg.pop(0)
	pc = applyCont
    elif tag == "identifier":
        id = token[1]
	sexp_reg = Symbol(id)
	tokens_reg.pop(0)
	pc = applyCont
    elif tag == "apostrophe":
	tokens_reg.pop(0)
	k_reg = ("quote-cont", k_reg)
	pc = parseSexp
    elif tag == "lparen":
	tokens_reg.pop(0)
	if isTokenType(tokens_reg[0], "dot"):
	    pc = parseError
        else:
	    terminator_reg = "rparen"
	    pc = parseSexpSequence
    elif tag == "lbracket":
	tokens_reg.pop(0)
	if isTokenType(tokens_reg[0], "dot"):
	    pc = parseError
        else:
	    terminator_reg = "rbracket"
	    pc = parseSexpSequence
    # new
    elif tag == "lvector":
        tokens_reg.pop(0)
        k_reg = ("vector-cont", k_reg)
        pc = parseVector
    else:
        pc = parseError

def parseSexpSequence():
    global tokens_reg, k_reg, terminator_reg, sexp_reg, pc
    token = tokens_reg[0]
    tag = token[0]
    if tag in ("rparen", "rbracket"):
        sexp_reg = ()
	pc = closeSexpSequence
    elif tag == "dot":
	tokens_reg.pop(0)
	k_reg = ("dot-cont", terminator_reg, k_reg)
	pc = parseSexp
    else:
	k_reg = ("seq1-cont", terminator_reg, k_reg)
	pc = parseSexp

def closeSexpSequence():
    global tokens_reg, k_reg, terminator_reg, sexp_reg, pc
    token = tokens_reg[0]
    tag = token[0]
    if tag in ("rparen", "rbracket"):
        if isTokenType(token, terminator_reg):
            tokens_reg.pop(0)
            pc = applyCont
        elif terminator_reg == "rparen":
            raise Exception("parenthesized list terminated by bracket")
        elif terminator_reg == "rbracket":
            raise Exception("bracketed list terminated by parenthesis")
        else:
            raise Exception("should never reach here")
    else:
        pc = parseError

# new
def parseVector():
    global tokens_reg, k_reg, sexp_reg, pc
    token = tokens_reg[0]
    tag = token[0]
    if tag == "rparen":
        sexp_reg = ()
        tokens_reg.pop(0)
        pc = applyCont
    else:
        k_reg = ("vector-sexp1-cont", k_reg)
        pc = parseSexp

def parseError():
    global tokens_reg
    token = tokens_reg[0]
    if isTokenType(token, "end-marker"):
        raise Exception("unexpected end of input")
    else:
        raise Exception("unexpected token %s encountered" % token)

# file loader

def loadFile(filename):
    global tokens_reg, pc
    tokens_reg = scanInput(readContent(filename))
    pc = processSexps
    return run()

def processSexps():
    global tokens_reg, sexp_reg, k_reg, pc
    token = tokens_reg[0]
    if isTokenType(token, "end-marker"):
	sexp_reg = Symbol("done")
	pc = None
    else:
	k_reg = ("process-cont",)
	pc = parseSexp

def readContent(filename):
    f = open(filename)
    content = f.read()
    f.close()
    return content

# continuations

def applyCont():
    global tokens_reg, k_reg, terminator_reg, sexp_reg, pc
    tag = k_reg[0]
    if tag == "init-cont":
        if isTokenType(tokens_reg[0], "end-marker"):
            pc = None
        else:
            raise Exception("tokens left over: %s" % tokens_reg)
    elif tag == "quote-cont":
        k = k_reg[1]
        k_reg = k
        sexp_reg = Cons(Symbol("quote"), Cons(sexp_reg, ()))
        pc = applyCont
    elif tag == "dot-cont":
        expectedTerminator, k = k_reg[1], k_reg[2]
	terminator_reg = expectedTerminator
	k_reg = k
	pc = closeSexpSequence
    elif tag == "seq1-cont":
        expectedTerminator, k = k_reg[1], k_reg[2]
	terminator_reg = expectedTerminator
	k_reg = ("seq2-cont", sexp_reg, k)
	pc = parseSexpSequence
    elif tag == "seq2-cont":
        sexp1, k = k_reg[1], k_reg[2]
	k_reg = k
	sexp_reg = Cons(sexp1, sexp_reg)
	pc = applyCont
    elif tag == "process-cont":
        prettyPrint(sexp_reg)
        pc = processSexps
    # new
    elif tag == "vector-cont":
        k = k_reg[1]
        k_reg = k
        sexp_reg = Vector(sexp_reg)
        pc = applyCont
    # new
    elif tag == "vector-sexp1-cont":
        k = k_reg[1]
        k_reg = ("vector-rest-cont", sexp_reg, k)
        pc = parseVector
    # new
    elif tag == "vector-rest-cont":
        sexp1, k = k_reg[1], k_reg[2]
        k_reg = k
        sexp_reg = Cons(sexp1, sexp_reg)
        pc = applyCont
    else:
        raise Exception("invalid continuation %s in applyCont" % k_reg)

#-----------------------------------------------------------------------
# S-expression representations

# changed

class Boolean:
    def __init__(self, bool):
        self.bool = bool

    def __repr__(self):
        if self.bool:
            return "#t"
        else:
            return "#f"

    def __eq__(self, other):
        return isinstance(other, Boolean) and self.bool == other.bool

    def __ne__(self, other):
        return not self.__eq__(other)


class Character:
    def __init__(self, char):
        self.char = char

    def __repr__(self):
        if self.char == '\0': return "#\\nul"
        if self.char == ' ': return "#\\space"
        if self.char == '\t': return "#\\tab"
        if self.char == '\n': return "#\\newline"
        if self.char == '\b': return "#\\backspace"
        if self.char == '\r': return "#\\return"
        if self.char == '\f': return "#\\page"
        else: return "#\\%c" % self.char    # not quite right

    def __eq__(self, other):
        return isinstance(other, Character) and self.char == other.char

    def __ne__(self, other):
        return not self.__eq__(other)


class String:
    def __init__(self, s):
        self.s = s

    def __repr__(self):
        return '"' + self.s + '"'

    def __eq__(self, other):
        return isinstance(other, String) and self.s == other.s

    def __ne__(self, other):
        return not self.__eq__(other)


class Symbol:
    def __init__(self, id):
        self.id = id

    def __repr__(self):
        return self.id

    def __eq__(self, other):
        return isinstance(other, Symbol) and self.id == other.id

    def __ne__(self, other):
        return not self.__eq__(other)


class Cons:
    def __init__(self, a, b):
        self.car = a
        self.cdr = b

    def __repr__(self):
        if self.car == Symbol("quote") and \
                isinstance(self.cdr, Cons) and \
                self.cdr.cdr == ():
            return "'%s" % (self.cdr.car,)
        else:
            s = "(%s" % (self.car,)
            sexp = self.cdr
            while isinstance(sexp, Cons):
                s += " %s" % (sexp.car,)
                sexp = sexp.cdr
            if sexp == ():
                s += ")"
            else:
                s += " . %s)" % (sexp,)
            return s


# () is represented as ()


class Vector:
    def __init__(self, consCell):
        self.elements = []
        while consCell != ():
            self.elements.append(consCell.car)
            consCell = consCell.cdr

    def __repr__(self):
        s = ""
        for element in self.elements:
            s += str(element) + " "
        return "#(" + s[:-1] + ")"

    def __eq__(self, other):
        return isinstance(other, Vector) and \
            self.elements == [] and other.elements == []

    def __ne__(self, other):
        return not self.__eq__(other)


# InexactNumber and ExactNumber should probably be subclasses of
# Number (which should be an abstract class?)

class InexactNumber:
    def __init__(self, value):
        self.value = float(value)

    def __repr__(self):
        return str(self.value)

    def __eq__(self, other):
        if isinstance(other, InexactNumber):
            return self.value == other.value
        elif isinstance(other, ExactNumber):
            return self.value == other.inexactValue()
        else:
            return False

    def __ne__(self, other):
        return not self.__eq__(other)

    def __add__(self, other):
        if isinstance(other, InexactNumber):
            return InexactNumber(self.value + other.value)
        elif isinstance(other, ExactNumber):
            return InexactNumber(self.value + other.inexactValue())
        else:
            raise Exception("+: %s is not a number" % other)

    def __sub__(self, other):
        if isinstance(other, InexactNumber):
            return InexactNumber(self.value - other.value)
        elif isinstance(other, ExactNumber):
            return InexactNumber(self.value - other.inexactValue())
        else:
            raise Exception("-: %s is not a number" % other)

    def __mul__(self, other):
        if isinstance(other, InexactNumber):
            return InexactNumber(self.value * other.value)
        elif isinstance(other, ExactNumber):
            return InexactNumber(self.value * other.inexactValue())
        else:
            raise Exception("*: %s is not a number" % other)

    def __div__(self, other):
        if isinstance(other, InexactNumber):
            return InexactNumber(self.value / other.value)
        elif isinstance(other, ExactNumber):
            return InexactNumber(self.value / other.inexactValue())
        else:
            raise Exception("/: %s is not a number" % other)


class ExactNumber:
    def __init__(self, num, den=1):
        if den == 0:
            raise Exception("cannot represent %d/%d" % (num, den))
        elif num * den < 0:
            sign = -1
        else:
            sign = +1
        num, den = abs(num), abs(den)
        # reduce to lowest terms
        a, b = num, den
        while b > 0:
            remainder = a % b
            a = b
            b = remainder
        gcd = a
        self.numerator = sign * num / gcd
        self.denominator = den / gcd
        
    def __repr__(self):
        if self.denominator == 1:
            return "%d" % self.numerator
        else:
            return "%d/%d" % (self.numerator, self.denominator)

    def inexactValue(self):
        return float(self.numerator) / float(self.denominator)

    def __eq__(self, other):
        if isinstance(other, ExactNumber):
            return self.numerator == other.numerator \
                and self.denominator == other.denominator
        elif isinstance(other, InexactNumber):
            return self.inexactValue() == other.value
        else:
            return False

    def __ne__(self, other):
        return not self.__eq__(other)

    def __add__(self, other):
        if isinstance(other, ExactNumber):
            n = self.numerator * other.denominator + other.numerator * self.denominator
            d = self.denominator * other.denominator
            return ExactNumber(n, d)
        elif isinstance(other, InexactNumber):
            return InexactNumber(self.inexactValue() + other.value)
        else:
            raise Exception("+: %s is not a number" % other)

    def __sub__(self, other):
        if isinstance(other, ExactNumber):
            n = self.numerator * other.denominator - other.numerator * self.denominator
            d = self.denominator * other.denominator
            return ExactNumber(n, d)
        elif isinstance(other, InexactNumber):
            return InexactNumber(self.inexactValue() - other.value)
        else:
            raise Exception("-: %s is not a number" % other)
   
    def __mul__(self, other):
        if isinstance(other, ExactNumber):
            n = self.numerator * other.numerator
            d = self.denominator * other.denominator
            return ExactNumber(n, d)
        elif isinstance(other, InexactNumber):
            return InexactNumber(self.inexactValue() * other.value)
        else:
            raise Exception("*: %s is not a number" % other)

    def __div__(self, other):
        if isinstance(other, ExactNumber):
            n = self.numerator * other.denominator
            d = self.denominator * other.numerator
            return ExactNumber(n, d)
        elif isinstance(other, InexactNumber):
            return InexactNumber(self.inexactValue() / other.value)
        else:
            raise Exception("/: %s is not a number" % other)


def prettyPrint(sexp):
    # not so pretty yet
    print sexp


#-----------------------------------------------------------------------
# examples:

# >>> parse("apple")
# >>> parse("#T")
# >>> parse("(a (b c (d)))")
# >>> parse("(a b c 1 2 -3.14 #f \"hello there\" #\\newline (e [f . x] . 4) ())")
# >>> loadFile("scanner-parser.ss")
# >>> parse("(a 'b (quote c) #(1 2 d))")
# >>> parse("2/3") + parse("3/4")
