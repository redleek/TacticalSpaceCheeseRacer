CS = mcs
CSDEBUG = -define:DEBUG
FILE = Program
NAME = TacticalSpaceCheeseRacer
DEBUGNAME = $(NAME)-DEBUG

all: $(NAME).exe

.PHONY: clean

clean:
	rm -f $(NAME).exe $(DEBUGNAME).exe

debug: $(FILE).cs
	$(CS) $(CSDEBUG) $(FILE).cs -out:$(DEBUGNAME).exe

$(NAME).exe: $(FILE).cs
	$(CS) $(FILE).cs -out:$(NAME).exe
