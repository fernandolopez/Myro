# setup.py
from distutils.core import setup
from distutils.file_util import copy_file
import sys
from myro import __VERSION__
#windows installer:
# python setup.py bdist_wininst
# patch distutils if it can't cope with the "classifiers" or
# "download_url" keywords
if sys.version < '2.2.3':
    from distutils.dist import DistributionMetadata
    DistributionMetadata.classifiers = None
    DistributionMetadata.download_url = None
setup(
    name="myro",
    description="My Robot Python Exploration Library, from the IPRE",
    version= __VERSION__,
    author="Doug Blank, and the IPRE",
    author_email="dblank@cs.brynmawr.edu",
    url="http://myro.roboteducation.org/",
    packages=['myro', 'myro.robots', 'myro.worlds', 'myro.globvars'],
    license="Shared Source",
    long_description="Tools for exploring robotics in education",
    classifiers = [
        'Development Status :: 5 - Production/Stable',
        'Intended Audience :: End Users/Desktop',
        'License :: Shared Source',
        'Natural Language :: English',
        'Operating System :: Microsoft :: Windows :: Mac',
        'Programming Language :: Python',
        'Topic :: Software Development :: Libraries',
    ],
)
import os
sitepath = []
idlepath = []
use_idle = True
try:
    import idlelib
    sitepath.append(idlelib.__file__.split(os.sep)[:-2])
    idlepath.append(idlelib.__file__.split(os.sep)[:-1])
    idlepath = os.sep.join(idlepath)
except ImportError:
    use_idle = False

sitepath.append("site-packages")
sitepath = os.sep.join(sitepath)
if "install" in sys.argv:
    if use_idle:
        copy_file("misc" + os.sep + "ScriptBinding.py", idlepath)
        copy_file("misc" + os.sep + "config-extensions.def", idlepath)

    copy_file("myro" + os.sep + "graphics.py", sitepath)
else:
    print "on install, will:"
    if use_idle:
        print "   copy misc" + os.sep + "ScriptBinding.py" + " -> "+ idlepath
        print "   copy misc" + os.sep + "config-extensions.def" + " -> " + idlepath
    print "   copy myro" + os.sep + "graphics.py" + " -> " + sitepath
    
