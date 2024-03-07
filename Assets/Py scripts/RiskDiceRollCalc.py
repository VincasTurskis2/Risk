aLoses2 = 0
dLoses2 = 0
bothLose = 0

for a1 in range(1, 7):
    for a2 in range(1, 7):
        for a3 in range(1, 7):
            for d1 in range(1, 7):
                for d2 in range(1, 7):
                    aLoses = 0
                    dLoses = 0
                    if max(a1, a2, a3) > max(d1, d2):
                        dLoses += 1
                    else:
                        aLoses += 1
                    if (a1+a2+a3)-(max(a1, a2, a3)+min(a1, a2, a3)) > min(d1, d2):
                        dLoses += 1
                    else:
                        aLoses += 1
                    if aLoses == 2:
                        aLoses2 += 1
                    elif dLoses == 2:
                        dLoses2 += 1
                    else:
                        bothLose += 1

print("Attacker loses 2: " + str(aLoses2)) #2275
print("Defender loses 2: " + str(dLoses2)) #2890
print("Both lose 1: " + str(bothLose)) # 2611
#Defender loses 1.17176 troops for every 1 attacker loss.