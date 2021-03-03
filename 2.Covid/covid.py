import sys
import collections
from tkinter import *
from tkinter import messagebox
from tkinter.ttk import *

from tkinter.messagebox import Message

from dateutil import relativedelta

import matplotlib.pyplot as plt
import urllib.request

from urllib.request import urlopen
from urllib.request import Request
from urllib.parse import urlencode
from urllib.parse import quote

from typing import List, Dict

import json

from datetime import datetime
from dateutil.relativedelta import relativedelta

class CovidApp:
    def __init__(self, root: Frame) -> None:
        style: Style = Style()
        
        self.root: Frame = root
        
        self.settings: Frame = Frame()
        control_names = ["Kraj", "Czas", "Przypadki", "Bierzące", "Śmierci", "Uzdrowienia"]
        # lista krajow
        self.countries: Combobox = Combobox(self.settings)
        self.countries.bind("<<ComboboxSelected>>", self.show_graph)
        self.countries.grid(column=1, row=1)
        self.countries['values'] = self.fetch_countries()
        
        # wybor czasu
        self.cmb_timerange: Combobox = Combobox(self.settings)
        self.cmb_timerange.bind("<<ComboboxSelected>>", self.show_graph)
        self.cmb_timerange.grid(column=1, row=2)
        self.cmb_timerange['values'] = ['Cały', 'Rok', '6 Miesięcy', '3 Miesiące', '1 miesiąc']


        self.intvar_show_cases = IntVar(value=1)
        self.chk_show_cases = Checkbutton(self.settings, variable=self.intvar_show_cases, command=self.show_graph)
        self.chk_show_cases.grid(column=1, row=3)

        self.intvar_show_active = IntVar(value=1)
        self.chk_show_active = Checkbutton(self.settings, variable=self.intvar_show_active, command=self.show_graph)
        self.chk_show_active.grid(column=1, row=4)

        self.intvar_show_deaths = IntVar()
        self.chk_show_deaths = Checkbutton(self.settings, variable=self.intvar_show_deaths, command=self.show_graph)
        self.chk_show_deaths.grid(column=1, row=5)

        self.intvar_show_recovered = IntVar()
        self.chk_show_recovered = Checkbutton(self.settings, variable=self.intvar_show_recovered, command=self.show_graph)
        self.chk_show_recovered.grid(column=1, row=6)


        self.lbl_summary = Label(self.settings)
        self.lbl_summary.grid(column=0, row=7, columnspan=2)
        # wykres
        self.plot1 = PhotoImage()

        self.lb_plot1 = Label(self.root, image=self.plot1)
        self.lb_plot1.pack(side=RIGHT, fill=BOTH)
    
        self.settings.pack(side=LEFT, fill=Y)
        
        
        for idx, i in enumerate(control_names):
            self.lbl_x: Label = Label(self.settings, text=i+":")
            self.lbl_x.grid(column=0, row=idx+1)

        # wykres startowy
        self.countries.set('Poland')
        self.cmb_timerange.current(0)
        self.show_graph()

        

    def show_summary(self):
        data = self.api_get('https://api.covid19api.com/summary')

        country = self.countries.get()
        summary = ""

        def create_summary(stats):
            return F'''
  Wszystkie:  {stats['TotalConfirmed'] / 1000} tys.
  Śmierci:  {stats['TotalDeaths'] / 1000} tys.
  Uzdrowienia:  {stats['TotalRecovered'] / 1000} tys.
'''

        summary += 'Świat:'
        summary += create_summary(data['Global'])
        summary += country + ':'
        summary += create_summary([i for i in data['Countries'] if i['Country'] == country][0])

        self.lbl_summary.config(text=summary)

    def api_get(self, url: str):
        request = Request(url, headers={'User-Agent': 'Mozilla/5.0'})
        try:
            data = urlopen(request)
        except Exception as e:
            print(e)
            print('url:', url)
            messagebox.showerror("Błąd", "Nastąpił błąd podczas łączenia się z api:\n" + str(e))
            return None

        data = json.loads(data.read())
        return data

    def show_graph(self, *args):
        print("Graph")

        self.show_summary()

        country = self.countries.get()

        time_table = {1: 12,
                      2: 6,
                      3: 3,
                      4: 1}

        idx = self.cmb_timerange.current()
        time_query = ''
        if idx >= 1:
            data_from = datetime.now() - relativedelta(months=time_table[idx])
            time_query = "?" + urlencode({'from': "{0}T00:00:00Z".format(data_from.strftime("%Y-%m-%d")),
                                    'to'  : "{0}T00:00:00Z".format(datetime.now().strftime("%Y-%m-%d"))})
            print(time_query)
            #time_query = "?from={0}T00:00:00Z&to={1}T00:00:00Z".format(data_from.strftime("%Y-%m-%d"), datetime.now().strftime("%Y-%m-%d"))

        
        plot_data = self.api_get("https://api.covid19api.com/total/country/" + quote(country) + time_query)
        if plot_data == None:
            return
        
        timeseries = [datetime.strptime(i["Date"].split("T")[0], "%Y-%m-%d") for i in plot_data]
        infections = [i["Confirmed"] for i in plot_data]
        active = [i["Active"] for i in plot_data]
        deaths = [i["Deaths"] for i in plot_data]
        recovered = [i["Recovered"] for i in plot_data]

        fig, ax = plt.subplots()

        ax.get_yaxis().get_major_formatter().set_scientific(False) # notacja podstawowa
        fig.autofmt_xdate() # obracanie tickow

        if self.intvar_show_cases.get():
            ax.plot(timeseries, infections, color='blue', label="Przypadki ogółem")
        if self.intvar_show_active.get():
            ax.plot(timeseries, active, color='C1', label="Aktywne przypadki")
        if self.intvar_show_deaths.get():
            ax.plot(timeseries, deaths, '--r', Label="Śmierci")
        if self.intvar_show_recovered.get():
            ax.plot(timeseries, recovered, '--g', Label="Uzdrowienia")
        
        

        ax.set_title("Zachorowania w czasie dla: " + country)

        leg = ax.legend()

        fig.savefig('plot.png')
        self.plot1.config(file="plot.png")
        
        


    def fetch_countries(self):
        request = Request("https://api.covid19api.com/summary", headers={'User-Agent': 'Mozilla/5.0'})
        
        try:
            url = urlopen(request)
        except Exception as e:
            print(e)
            messagebox.showerror("Błąd", "Nastąpił błąd podczas łączenia się z api:\n" + str(e))
            self.root.destroy()
            sys.exit()
        #except 
        data = json.loads(url.read())

        countries = [i['Country'] for i in data['Countries']]
        countries.sort()

        return countries
        

root : Tk = Tk()

root.title('CovidApp')
root.iconbitmap('icon.ico')

covidApp = CovidApp(root)
root.mainloop()
