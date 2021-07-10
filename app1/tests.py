import sys
from django.test import TestCase
from time import sleep

cow = """
 _____________
< Test Start! >
 -------------
        \   ^__^
         \  (oo)\_______
            (__)\       )\/\\
                ||----w |
                ||     ||
"""

class SampleTestCase(TestCase):
    def setUp(self):
        sleep(0.5)

    def tearDown(self):
        sleep(0.5)

    def test_example(self):
        print(cow)
        print("Python{}".format(sys.version))

