LICZBY_RZYMSKIE = {
    1000: 'M',
    900: 'CM',
    500: 'D',
    400: 'CD',
    100: 'C',
    90: 'XC',
    50: 'L',
    40: 'XL',
    10: 'X',
     9: 'IX',
     5: 'V',
     4: 'IV',
     1: 'I'
}

def konwertuj_na_rzymskie(n : int) -> str:
    liczba_rzymska = ''
    for k, v in LICZBY_RZYMSKIE.items():
        while n >= k:
            liczba_rzymska += v
            n -= k
    return liczba_rzymska

def konwerter():
    i = input('Podaj liczbę: ')

    if not i.isdigit():
        print('Liczba nie jest liczbą całkowitą dodatnią')
        return

    i = int(i)
    if i > 4999:
        print('Liczba jest za duza (max: 4999)')
        return

    print(konwertuj_na_rzymskie(i))
    
konwerter()
